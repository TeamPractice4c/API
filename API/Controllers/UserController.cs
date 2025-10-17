using API.Enums;
using API.ExportClasses;
using API.InternalClasses;
using API.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController(PostgresContext context) : ControllerBase
    {
        private readonly PostgresContext Context = context;

        [HttpGet("GetUsers")]
        public ActionResult<List<ExportUser>> GetUsers()
        {
            
            List<User> users = [.. Context.Users];

            if (users is null || users.Count == 0)
            {
                return NotFound();
            }
            List<ExportUser> response = [];

            users.ForEach(x => response.Add(x.ToExport()));

            return Ok(response);
        }

        [HttpGet("GetUser/{id}")]
        public ActionResult<ExportUser> GetUser(int id)
        {
            User? user = Context.Users.FirstOrDefault(x => x.UId == id);

            if (user is null)
            {
                return NotFound("Пользователь не найден");
            }

            return Ok(user.ToExport());
        }

        [HttpDelete("DeleteUser/{id}")]
        public ActionResult DeleteUser(int id)
        {
            User? user = Context.Users.FirstOrDefault(x => x.UId == id);

            if (user is null)
            {
                return NotFound("Пользователь не найден");
            }

            Context.Users.Remove(user);

            Context.SaveChanges();

            return Ok();
        }


        [HttpPost("Auth")]
        public ActionResult<ExportUser> Authorization([FromForm] string login, [FromForm] string password)
        {
            User? user = Context.Users.FirstOrDefault(x => x.UEmail == login && x.UPassword == password);

            if (user is null)
            {
                return BadRequest("Неверный логин или пароль");
            }

            return Ok(user.ToExport());
        }

        [HttpPost("Register")]
        public ActionResult<ExportUser> Register([FromBody] ExportUser user)
        {
            if (Context.Users.Any(x => x.UEmail == user.UEmail))
            {
                return BadRequest("Указанный логин занят");
            }

            int id = Context.Users.Any() ? Context.Users.Max(x => x.UId) + 1 : 1;

            PasswordHasher<ExportUser> hasher = new();
            string enc_password = hasher.HashPassword(user, user.UPassword);
            User new_user = new()
            {
                UId = id,
                UName = user.UName,
                USurname = user.USurname,
                UPatronymic = user.UPatronymic,
                UEmail = user.UEmail,
                UPassword = enc_password,
                URole = (Role)Convertation.ConvertStringToEnum<Role>("Клиент")!,
                UPhone = user.UPhone,
                UBirthdate = user.UBirthdate,
                UPassportNumber = user.UPassportNumber,
                UPassportSerial = user.UPassportSerial,
            };

            Context.Users.Add(new_user);

            Context.SaveChanges();

            return Ok(new_user.ToExport());
        }

        [HttpPost("ChangePassword")]
        public ActionResult<ExportUser> ChangeUserPassword([FromBody] ExportUser user)
        {
            User? gotten_user = Context.Users.FirstOrDefault(x => x.UId == user.UId);

            if (gotten_user is null)
            {
                return BadRequest("Пользователь не найден");
            }

            PasswordHasher<ExportUser> hasher = new();
            string new_password = hasher.HashPassword(gotten_user.ToExport(), user.UPassword);

            gotten_user.UPassword = new_password;

            Context.Users.Update(gotten_user);
            Context.SaveChanges();

            return Ok(gotten_user.ToExport());
        }

        [HttpPost("EditUser")]
        public ActionResult<ExportUser> EditUserInfo([FromBody] ExportUser user)
        {
            User? gotten_user = Context.Users.FirstOrDefault(x => x.UEmail == user.UEmail);

            if (gotten_user is null)
            {
                return BadRequest("Пользователь не найден");
            }

            gotten_user.UName = user.UName;
            gotten_user.USurname = user.USurname;
            gotten_user.UPatronymic = user.UPatronymic;
            gotten_user.UPhone = user.UPhone;
            gotten_user.UEmail = user.UEmail;
            gotten_user.UPassportSerial = user.UPassportSerial;
            gotten_user.UPassportNumber = user.UPassportNumber;
            gotten_user.UBirthdate = user.UBirthdate;


            Context.Users.Update(gotten_user);
            Context.SaveChanges();

            return Ok(gotten_user.ToExport());
        }

/*        [HttpPost("Upload")]
        [RequestSizeLimit(10_000_000)]
        public async Task<IActionResult> Upload([FromForm] UploadFile file)
        {
            if (file.File == null || file.File.Length == 0)
            {
                return BadRequest("Файл изображения не передан или пустой.");
            }

            var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images", file.File.FileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.File.CopyToAsync(stream);
            }

            return Ok();
        }*/
    }
}
