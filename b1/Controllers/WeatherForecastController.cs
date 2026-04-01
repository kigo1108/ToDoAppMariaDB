using b1.ToDo;
using Microsoft.AspNetCore.Mvc;

namespace b1.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly ITodoService _todoService;
        public WeatherForecastController(ILogger<WeatherForecastController> logger, ITodoService todoService)
        {
            _logger = logger;
            _todoService = todoService;
        }
        /// <summary>
        /// API lấy danh sách việc cần làm của Nam
        /// </summary>
        [HttpGet("todos")]
        public ActionResult<IEnumerable<string>> GetTodos()
        {
            var a = _todoService.GetAllTodosDtoAsync();
            return Ok(a);
        }
        /// <summary>
        /// API tra ve hello
        /// </summary>
        [HttpGet("hi")]
        public IActionResult Hello(string name)
        {
            return Ok($"Hello {name}");
        }

        /// <summary>
        /// Api them cong viec moi vao danh sach
        /// </summary>
        /// <param name="todoName">Ten cong viec can them</param>
        [HttpPost("add-todo")]
        public IActionResult AddTodo(string todoName)
        {

            return Ok($"Added todo: {todoName}");
        }
    }
}
