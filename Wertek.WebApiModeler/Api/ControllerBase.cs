using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Wertek.WebApiModeler.ExtensionMethods;
using Wertek.WebApiModeler.Models;

namespace Wertek.WebApiModeler.Api;

public class ControllerBase<TObject, TEntity> : ControllerBase 
    where TObject : class
    where TEntity : class
{
    protected readonly DbContext _dbContext;
    protected readonly IMapper _mapper;
    public ControllerBase(
        DbContext dbContext,
        IMapper mapper
    )
    {
        _dbContext = dbContext;
        
        _mapper = mapper;
    }

    [HttpGet]
    public virtual async Task<ActionResult<IEnumerable<TObject>>> GetAll()
    {
        IQueryable<TObject> query = _dbContext.Set<TEntity>()
        .ProjectTo<TObject>(_mapper.ConfigurationProvider);

        var entities = await query.ToListAsync();
        
        return Ok(entities);
    }

    [HttpPost("paginated")]
    public virtual async Task<ActionResult<PaginatedResult<TObject>>> GetAllPaginated(FilterDTO? filter)
    {
        return Ok(await this.GetAllPaginatedBase<TObject, TEntity>(filter));
    }

    public async Task<PaginatedResult<Tobj>> GetAllPaginatedBase<Tobj, Tent>(FilterDTO? filter)  where Tobj : class where Tent : class {
        IQueryable<Tobj> query = _dbContext.Set<Tent>()
            .ProjectTo<Tobj>(_mapper.ConfigurationProvider);

        query = query.ToFilterView(filter);
        var entities = await query.ToListAsync();
        var count = await _dbContext.Set<Tent>().CountAsync();

        var totalFiltered = await _dbContext.Set<Tent>()
            .ProjectTo<Tobj>(_mapper.ConfigurationProvider)
            .ToFilterView(new FilterDTO{Filters = filter != null ? filter.Filters: new List<Filter>()})
            .CountAsync();
        var result = new PaginatedResult<Tobj>
        {
            Page = filter != null ? filter.Page : 1,
            PageSize = filter != null ? filter.PageSize : 0,
            TotalCount = count,
            TotalFiltered = totalFiltered,
            TotalPages = filter != null && filter.PageSize > 0 ? (int)Math.Ceiling((double)totalFiltered / filter.PageSize) : 1,
            Items = entities
        };

        return result;
    }

    private bool HasDependents(TEntity entity) {
        var entityType = typeof(TEntity);
        var properties = entityType.GetProperties();
        foreach (var property in properties)
        {
            var propertyType = property.PropertyType;
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
            {
                var collection = property.GetValue(entity);
                if (collection != null)
                {
                    var collectionType = collection.GetType();
                    var countProperty = collectionType.GetProperty("Count");
                    var count = (int)countProperty!.GetValue(collection)!;
                    if (count > 0)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    [HttpPost("values-for/{field}")]
    public virtual async Task<ActionResult<IEnumerable<object>>> ValuesFor(
        [FromRoute] string field,
        [FromBody] FilterValueFor filters)
    {       
        return Ok(await ValuesForBase<TObject, TEntity>(field, filters));
    }
    public async Task<IEnumerable<object>> ValuesForBase<Tobj,Tent>(
        string field,
        FilterValueFor filters) where Tobj : class where Tent : class
    {
        var filterList = filters.Filters.ToList();
        filterList.Add(new Filter
        {
            Clauses = new List<Clause>
            {
                new Clause
                {
                    Field = field,
                    Operator = Operators.Contains,
                    Value = filters.AutoComplete
                }
            },
            Logic = Logic.And
        });
        filters.Filters = filterList;
        var filter = new FilterDTO
        {
            Page = 0,
            PageSize = 0,
            Filters = filters.Filters,
            Sort = new List<Sort>
            {
                new Sort
                {
                    Dir = filters.Direction,
                    Field = field
                }
            }
        };
        
        IQueryable<Tobj> query = _dbContext.Set<Tent>()
            .ProjectTo<Tobj>(_mapper.ConfigurationProvider);

        query = query.ToFilterView(filter);
        var entities = await query
            
            .ToListAsync();
            
        return entities.Select(x => new {
                Key = x.GetType().GetProperty(string.IsNullOrEmpty(filters.KeyField.Capitalize()) ? field : filters.KeyField.Capitalize())!.GetValue(x),
                Value = x.GetType().GetProperty(field.Capitalize())!.GetValue(x)
            }).Distinct();
    }

    [HttpPost("export")]
    public virtual async Task<IActionResult> GetExcelFile(FilterDTO? filter)
    {
        return await GetExcelFileBase<TObject, TEntity>(filter);
    }

    public async Task<FileContentResult> GetExcelFileBase<Tobj,Tent>(FilterDTO? filter) where Tobj : class where Tent : class
    {
        IQueryable<Tent> query = _dbContext.Set<Tent>();

        query = query.ToFilterView(filter);
        var entities = await query.ToListAsync();

        var models = _mapper.Map<IEnumerable<Tobj>>(entities);

        using (var package = new ExcelPackage())
        {
            var worksheet = package.Workbook.Worksheets.Add("Sheet1");
            worksheet.Cells.LoadFromCollection(models, true);

            var fileContents = package.GetAsByteArray();
            var fileName = "PexProExport.xlsx";
            return File(fileContents, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }

    [HttpGet("{id}")]
    public virtual async Task<ActionResult<TObject>> GetById([FromRoute] long id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }
        var model = _mapper.Map<TObject>(entity);
        return Ok(model);
    }

    [HttpGet("{id}/has-dependents")]
    public virtual async Task<ActionResult<bool>> GetHasDependents([FromRoute] long id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }
        return Ok(HasDependents(entity));
    }

    [HttpPost]
    public virtual async Task<ActionResult<TObject>> Insert([FromBodyAttribute] TObject model)
    {
        var entity = _mapper.Map<TEntity>(model);
        _dbContext.Set<TEntity>().Add(entity);
        await _dbContext.SaveChangesAsync();
        var insertedModel = _mapper.Map<TObject>(entity);
        var keyName = _dbContext.Model.FindEntityType(typeof(TEntity))?.FindPrimaryKey()?.Properties
            ?.Select(x => x.Name)?.SingleOrDefault();
        var idProperty = typeof(TEntity).GetProperty(keyName ?? "");
        var idValue = idProperty?.GetValue(entity);
        return CreatedAtAction(nameof(GetById), new { id = idValue }, insertedModel);
    }

    [HttpPut("{id}")]
    public virtual async Task<ActionResult<TObject>> Update([FromRoute] long id, [FromBodyAttribute] TObject model)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }
        _mapper.Map(model, entity);
        await _dbContext.SaveChangesAsync();
        var updatedModel = _mapper.Map<TObject>(entity);
        return Ok(updatedModel);
    }

    [HttpDelete("{id}")]
    public virtual async Task<ActionResult> Delete([FromRoute] long id)
    {
        var entity = await _dbContext.Set<TEntity>().FindAsync(id);
        if (entity == null)
        {
            return NotFound();
        }
        _dbContext.Set<TEntity>().Remove(entity);
        await _dbContext.SaveChangesAsync();
        return NoContent();
    }
}