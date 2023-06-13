using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml;
using Wertek.WebApiModeler.ExtensionMethods;
using Wertek.WebApiModeler.Models;

namespace Wertek.WebApiModeler.Api;

public class ControllerBase<TObject, TList, TEntity> : ControllerBase 
    where TObject : class
    where TList : class
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
    public virtual async Task<ActionResult<IEnumerable<TList>>> GetAll()
    {
        IQueryable<TList> query = _dbContext.Set<TEntity>()
        .ProjectTo<TList>(_mapper.ConfigurationProvider);

        var entities = await query.ToListAsync();
        
        return Ok(entities);
    }

    [HttpPost("paginated")]
    public virtual async Task<ActionResult<PaginatedResult<TList>>> GetAllPaginated(FilterDTO? filter)
    {
        IQueryable<TList> query = _dbContext.Set<TEntity>()
            .ProjectTo<TList>(_mapper.ConfigurationProvider);

        query = query.ToFilterView(filter);
        var entities = await query.ToListAsync();
        var count = await _dbContext.Set<TEntity>().CountAsync();

        var totalFiltered = await _dbContext.Set<TEntity>()
            .ProjectTo<TList>(_mapper.ConfigurationProvider)
            .ToFilterView(new FilterDTO{Filters = filter != null ? filter.Filters: new List<Filter>()})
            .CountAsync();
        var result = new PaginatedResult<TList>
        {
            Page = filter != null ? filter.Page : 0,
            PageSize = filter != null ? filter.PageSize : 0,
            TotalCount = count,
            TotalFiltered = totalFiltered,
            TotalPages = filter != null ? count / filter.PageSize : 0,
            Items = entities
        };
        return Ok(result);
    }

    [HttpPost("values-for/{field}")]
    public virtual async Task<ActionResult<IEnumerable<object>>> ValuesFor(
        [FromRoute] string field,
        [FromBody] FilterValueFor filters)
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
        IQueryable<TList> query = _dbContext.Set<TEntity>()
            .ProjectTo<TList>(_mapper.ConfigurationProvider);

        query = query.ToFilterView(filter);
        var entities = await query
            .Select(x => new {
                Key = x.GetType().GetProperty(string.IsNullOrEmpty(filters.KeyField) ? field : filters.KeyField)!.GetValue(x),
                Value = x.GetType().GetProperty(field)!.GetValue(x)
            })
            .ToListAsync();
            
        return Ok(entities.Distinct());
    }

    [HttpPost("export")]
    public virtual async Task<IActionResult> GetExcelFile(FilterDTO? filter)
    {
        IQueryable<TEntity> query = _dbContext.Set<TEntity>();

        query = query.ToFilterView(filter);
        var entities = await query.ToListAsync();

        var models = _mapper.Map<IEnumerable<TList>>(entities);

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