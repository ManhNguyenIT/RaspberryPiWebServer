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
        private readonly Settings _settings;

        public HistoryController(IEntityService<History> service, MonitorService monitorService)
        {
            _service = service;
            _monitorService = monitorService;
            _settings = monitorService.Read();
        }

        [HttpGet]
        public object Get(DataSourceLoadOptions loadOptions)
        {
            return DataSourceLoader.Load(_service.GetAll(), loadOptions);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromForm] string values)
        {
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.Date.AddDays(1);
            try
            {
                var item = new Item();
                JsonConvert.PopulateObject(values, item);
                var _item = _settings.Outputs.Where(i => i.Name == item.Name && i.Pin == item.Pin).FirstOrDefault();
                if (_item == null)
                {
                    return BadRequest($"Cannot find Output with name {_item.Name} and pin {_item.Pin}");
                }

                await _monitorService.WriteAsync(_item, _settings.Delay);

                var entity = new History();
                JsonConvert.PopulateObject(values, entity);

                if (entity.isCount)
                {
                    await _service.Add(entity);
                }

                var querry = _service.GetAll().Where(i => i.Template == entity.Template && i.Model == entity.Model && from < i.Created && i.Created < to);

                return Ok(new { Ok = querry.Where(i => i.Code == i.Template), Ng = querry.Where(i => i.Code != i.Template) });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("current")]
        public IActionResult Current([FromBody] History history)
        {
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.Date.AddDays(1);
            try
            {
                var querry = _service.GetAll().Where(i => i.Template == history.Template && i.Model == history.Model && from < i.Created && i.Created < to);
                return Ok(new { Ok = querry.Where(i => i.Code == i.Template).Count(), Ng = querry.Where(i => i.Code != i.Template).Count() });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("report")]
        public async Task<object> Report(DataSourceLoadOptions loadOptions)
        {
            DateTime from = DateTime.Now.Date;
            DateTime to = DateTime.Now.Date.AddDays(1);
            try
            {
                var table = _service.GetAll().Where(i => from < i.Created && i.Created < to);

                var querry = table.GroupBy(i => new { i.Code, i.Template })
                    .Select(i => new { i.Key.Code, i.Key.Template, StartAt = i.Min(g => g.Created) });

                var table2 = from i in table
                             join q in querry on new { i.Code, i.Template } equals new { q.Code, q.Template }
                             select new { i.Code, i.Template, q.StartAt };

                var res = (await table2.ToListAsync())
                        .GroupBy(i => new { i.Code, i.Template, i.StartAt })
                        .Select(i => new Report() { Code = i.Key.Code, Template = i.Key.Template, StartAt = i.Key.StartAt, Total = i.Count(), Ok = i.Select(h => h.Code == h.Template).Count() }).ToList();

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
