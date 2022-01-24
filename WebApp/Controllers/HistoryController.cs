using DevExtreme.AspNet.Data;
using DevExtreme.AspNet.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApp.Models;
using WebApp.Services;

namespace WebApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HistoryController : ControllerBase
    {
        private readonly IEntityService<History> _service;
        private readonly MonitorService _monitorService;
        private readonly Item[] _outputs;

        public HistoryController(IEntityService<History> service, MonitorService monitorService)
        {
            _service = service;
            _monitorService = monitorService;
            _outputs = monitorService.GetOutputs();
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            return DataSourceLoader.Load(_service.GetAll(), loadOptions);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string values)
        {
            try
            {
                var entity = new History();
                JsonConvert.PopulateObject(values, entity);
                entity= await _service.Add(entity);
                return Ok(entity);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        private async Task<dynamic> GetCurrent(History history)
        {
            DateTime date = DateTime.Now.Date;
            var last = await _service.GetAll().Where(i => !i.IsCancelled)
                    .OrderByDescending(i => i.Created).FirstOrDefaultAsync();
            if (last == null || last.Model!=history.Model||last.Template!=history.Template||last.Created<date)
            {
                return new { Ok = 0, Ng = 0 };
            }

            var querry = _service.GetAll()
                .Where(i => i.Template == history.Template && i.Model == history.Model && i.Times==last.Times && date < i.Created);
            return new { Ok = querry.Where(i => i.Code == i.Template).Count(), Ng = querry.Where(i => i.Code != i.Template).Count() };
        }

        [HttpPost("current")]
        public async Task<IActionResult> Current([FromBody] History history)
        {
            return Ok(await GetCurrent(history));
        }

        [HttpPost("cancel-lastest")]
        public async Task<IActionResult> CancelLastest([FromBody] History history)
        {
            DateTime date = DateTime.Now.Date;
            try
            {
                var entity = await _service.GetAll()
                    .OrderByDescending(i => i.Created).FirstOrDefaultAsync();
                if (entity == null||entity.Model!=history.Model||entity.Template!=history.Template||entity.Created<date)
                {
                    return NotFound();
                }
                if (entity.IsCancelled)
                {
                    return NoContent();
                }
                entity.IsCancelled=true;
                entity.Updated = DateTime.Now;
                await _service.Update(entity);

                return Ok(await GetCurrent(history));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("post")]
        public async Task<IActionResult> PostAsync([FromBody] History history)
        {
            DateTime date = DateTime.Now.Date;
            try
            {
                var _item = _outputs.Where(i => i.Name == history.Name && i.Pin == history.Pin).FirstOrDefault();
                if (_item == null)
                {
                    return BadRequest($"Output with name {history.Name} and pin {history.Pin} not found");
                }

                await _monitorService.WriteAsync(_item);

                var last = await _service.GetAll()
                    .OrderByDescending(i => i.Created).FirstOrDefaultAsync();
                if (last == null || last.Created.Date<date)
                {
                    history.Times=1;
                }
                else if (last.Model!=history.Model||last.Template!=history.Template)
                {
                    history.Times+=1;
                }
                else
                {
                    history.Times=last.Times;
                }
                await _service.Add(history);

                return Ok(await GetCurrent(history));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("report")]
        public async Task<object> Report(DataSourceLoadOptions loadOptions)
        {
            try
            {
                var table = _service.GetAll().Where(i => !i.IsCancelled);
                var querry = table.GroupBy(i => new { i.Template, i.Model, i.Times, i.Created.Date })
                    .Select(i => new { i.Key.Template, i.Key.Model, i.Key.Times, i.Key.Date, StartAt = i.Min(g => g.Created), Ok = i.Where(g => g.Code==g.Template).Count(), Total = i.Count() });
                var table2 = from i in table
                             join q in querry on new { i.Template, i.Model, i.Times, i.Created.Date } equals new { q.Template, q.Model, q.Times, q.Date }
                             select new { i.Template, i.Model, i.Times, q.Date, q.StartAt, q.Ok, q.Total };

                var res = await table2.GroupBy(i => new { i.Template, i.Model, i.Times, i.Date, i.StartAt })
                        .Select(i =>
                        new Report()
                        {
                            Template = i.Key.Template,
                            Model=i.Key.Model,
                            Date=i.Key.Date,
                            StartAt = i.Key.StartAt,
                            Total = i.Max(g => g.Total),
                            Ok = i.Max(g => g.Ok)
                        }).ToListAsync();

                return DataSourceLoader.Load(res, loadOptions);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Put([FromForm] Guid key, [FromForm] string values)
        {
            var entity = await _service.Get(key);
            if (entity == null)
            {
                return NotFound();
            }
            JsonConvert.PopulateObject(values, entity);

            try
            {
                await _service.Update(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid key)
        {
            var entity = await _service.Get(key);
            if (entity == null)
            {
                return NotFound();
            }

            try
            {
                await _service.Delete(entity);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
