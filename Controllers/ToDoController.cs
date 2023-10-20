using DotNetCoreAssignments.Models;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ToDoController : ControllerBase
    {
        private readonly ToDoRepository _repository;

        public ToDoController(ToDoRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // GET: api/<ToDoController>
        [HttpGet]
        public ActionResult<IEnumerable<ToDo>> GetAll()
        {
            var ToDos = _repository.GetAll();
            return Ok(ToDos);
        }

        // GET api/<ToDoController>/id
        [HttpGet("{id}")]
        public ActionResult<ToDo> GetById(Guid id)
        {
            var item = _repository.GetById(id);
            if (item == null)
            {
                return NotFound("Item not found");
            }
            return Ok(item);
        }

        // POST api/<ToDoController>
        [HttpPost]
        public ActionResult<ToDo> Post([FromBody] ToDo item)
        {
            if (item == null)
            {
                return BadRequest("Invalid data");
            }

            try
            {
                var addedItem = _repository.Create(item);

                return CreatedAtAction(nameof(GetById), new { id = addedItem.Id }, item);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request. Please try again later." + ex.Message);
            }
        }


        // PUT api/<ToDoController>/5
        [HttpPut("{id}")]
        public IActionResult Put(Guid id, [FromBody] ToDo item)
        {
            var existingItem = _repository.GetById(id);
            
            if (existingItem == null)
            {
                return NotFound("Item not found");
            }

            try {
                item.Id = id;
                _repository.Update(item);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound("Item not found");
            }
        }

        // DELETE api/<ToDoController>/5
        [HttpDelete("{id}")]
        public IActionResult Delete(Guid id)
        {
            try
            {
                var existingItem = _repository.GetById(id);

                if (existingItem == null)
                {
                    return NotFound("Todo not found");
                }

                _repository.Delete(id);
                return Ok();

            }
            catch (Exception ex)
            {
                return StatusCode(500, "An error occurred while processing the request. Please try again later." + ex.Message);
            }
        }

    }
}
