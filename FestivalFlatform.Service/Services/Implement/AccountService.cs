using System;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FestivalFlatform.Data.Models;
using FestivalFlatform.Data.UnitOfWork;
using FestivalFlatform.Service.DTOs.Request;
using FestivalFlatform.Service.DTOs.Response;
using FestivalFlatform.Service.Exceptions;
using FestivalFlatform.Service.Services.Interface;
using Microsoft.AspNetCore.Identity;
using Azure.Core;
using Microsoft.AspNetCore.Http;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using System.Web;
using FestivalFlatform.Service.Helpers;

namespace FestivalFlatform.Service.Services.Implement
{
    public class AccountService : IAccountService
    {
        private readonly IUnitOfWork _unitOfWork;
        private IMapper _mapper;
        private readonly IConfiguration _config;

        public AccountService(IMapper mapper, IUnitOfWork unitOfWork, IConfiguration config)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = config;
        }


       
        private void CreatePasswordHash(string password, out string passwordHash)
        {
            passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public async Task<AccountResponse> RegisterAccount(RegisterRequestAll request)
        {
          
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);
            if (emailExisted != null)
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", request.Email);

            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == request.PhoneNumber);
            if (phoneNumberExisted != null)
                throw new CrudException(HttpStatusCode.Conflict, "Số điện thoại đã tồn tại", request.PhoneNumber);

          
            CreatePasswordHash(request.Password, out string passwordHash);

            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = request.RoleId,
                Status = request.Status ?? false,
                ClassName = string.IsNullOrWhiteSpace(request.ClassName) ? null : request.ClassName.Trim(),
                PasswordHash = passwordHash,
                PlainPassword = request.Password, 
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            
            var wallet = new Wallet
            {
                AccountId = account.AccountId,
                Balance = 0,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullName = account.FullName,
                ClassName = account.ClassName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
                Status = account.Status,
                WalletId = wallet.WalletId
            };
        }


        public async Task<AccountResponse> RegisterStudentAccountBySchoolManager(RegisterRequest request)
        {
            
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == request.Email);
            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", request.Email.ToString());
            }

           
            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == request.PhoneNumber);
            if (phoneNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Số điện thoại đã tồn tại");
            }

          
            var studentRole = await _unitOfWork.Repository<Role>()
                .GetAll()
                .FirstOrDefaultAsync(r => r.RoleName.ToLower() == "student");

            if (studentRole == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Không tìm thấy role 'student'");
            }

           
            CreatePasswordHash(request.Password, out string passwordHash);

          
            var account = new Account
            {
                FullName = request.FullName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                RoleId = studentRole.RoleId,
                PasswordHash = passwordHash,
                PlainPassword = request.Password,  
                ClassName = string.IsNullOrWhiteSpace(request.ClassName) ? null : request.ClassName.Trim(),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<Account>().InsertAsync(account);
            await _unitOfWork.CommitAsync();

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                FullName = account.FullName,
                ClassName = account.ClassName,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                Status = account.Status,
                CreatedAt = account.CreatedAt,
                
            };
        }


        public async Task<List<AccountResponse>> GetAccount(int? id, string? phone, string? email, string? classNAme, int? role, int? pageNumber, int? pageSize)
        {
            var query = _unitOfWork.Repository<Account>().GetAll()

                                                         .Where(a => !id.HasValue || id == 0 || a.AccountId == id.Value)

                                                         .Where(a => string.IsNullOrWhiteSpace(phone) || a.PhoneNumber.Contains(phone.Trim()))
                                                         .Where(a => string.IsNullOrWhiteSpace(classNAme) || a.ClassName.Contains(classNAme.Trim()))

                                                         .Where(a => string.IsNullOrWhiteSpace(email) || a.Email.Contains(email.Trim()))
                                                         .Where(a => !role.HasValue || role == 0 || a.RoleId == role.Value);

            //int currentPage = pageNumber.HasValue && pageNumber.Value > 0 ? pageNumber.Value : 1;
            //int currentSize = pageSize.HasValue && pageSize.Value > 0 ? pageSize.Value : 10;

            //query = query.Skip((currentPage - 1) * currentSize)
            //             .Take(currentSize);

            var accounts = await query.ToListAsync();


            return accounts.Select(account => new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                Pasword = account.PasswordHash,
                FullName = account.FullName,
                ClassName = account.ClassName,
                PhoneNumber = account.PhoneNumber,
                AvatarUrl = account.AvatarUrl,
                RoleId = account.RoleId,
                Status = account.Status,
                CreatedAt = account.CreatedAt
            }).ToList();
        }

        public async Task DeleteAccountAsync(int accountId)
        {
            var account = _unitOfWork.Repository<Account>().Find(a => a.AccountId == accountId);
            if (account == null)
            {
                throw new KeyNotFoundException("Không tìm thấy tài khoản.");
            }

            _unitOfWork.Repository<Account>().Delete(account);
            await _unitOfWork.CommitAsync();
        }

        public async Task<AccountResponse> UpdateAccount(int id, AccountUpdateRequest accountRequest)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            if (account == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", id.ToString());
            }
            var emailExisted = _unitOfWork.Repository<Account>().Find(x => x.Email == accountRequest.Email);

            if (emailExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Email đã tồn tại", accountRequest.Email.ToString());
            }

            var phoneNumberExisted = _unitOfWork.Repository<Account>().Find(x => x.PhoneNumber == accountRequest.PhoneNumber);

            if (phoneNumberExisted != null)
            {
                throw new CrudException(HttpStatusCode.Conflict, "so dien thoai đã tồn tại", accountRequest.PhoneNumber.ToString());
            }


            if (!string.IsNullOrWhiteSpace(accountRequest.FullName))
                account.FullName = accountRequest.FullName;

            if (!string.IsNullOrWhiteSpace(accountRequest.ClassName))
                account.ClassName = accountRequest.ClassName;

            if (!string.IsNullOrWhiteSpace(accountRequest.Email))
                account.Email = accountRequest.Email;

            if (!string.IsNullOrWhiteSpace(accountRequest.Pasword))
            {
                CreatePasswordHash(accountRequest.Pasword, out string passwordHash);
                account.PasswordHash = passwordHash;
            }

            if (!string.IsNullOrWhiteSpace(accountRequest.PhoneNumber))
                account.PhoneNumber = accountRequest.PhoneNumber;

            if (!string.IsNullOrWhiteSpace(accountRequest.AvatarUrl))
                account.AvatarUrl = accountRequest.AvatarUrl;

            if (accountRequest.Status.HasValue)
                account.Status = accountRequest.Status.Value;

            account.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.Repository<Account>().Update(account, id);
            await _unitOfWork.CommitAsync();


            account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(o => o.Role)
                .FirstOrDefaultAsync(a => a.AccountId == id);

            return new AccountResponse
            {
                Id = account.AccountId,
                Email = account.Email,
                FullName = account.FullName,
                ClassName = account.ClassName,
                Pasword = account.PasswordHash,
                PhoneNumber = account.PhoneNumber,
                RoleId = account.RoleId,
                CreatedAt = account.CreatedAt,
                UpdatedAt = account.UpdatedAt,

            };
        }




        public async Task<(List<AccountResponse> successAccounts, List<string> errors)> ImportAccountsFromExcelAsync(int schoolId, Stream excelStream)
        {
            List<AccountResponse> createdAccounts = new();
            List<string> errors = new();

            IWorkbook workbook = new XSSFWorkbook(excelStream);
            ISheet sheet = workbook.GetSheetAt(0);

            for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++) 
            {
                IRow? row = sheet.GetRow(rowIndex);
                if (row == null) continue;

                string? fullName = row.GetCell(0)?.ToString()?.Trim();
                string? phoneNumber = row.GetCell(1)?.ToString()?.Trim();
                string? className = row.GetCell(2)?.ToString()?.Trim();   
                string? email = row.GetCell(3)?.ToString()?.Trim();
                string? password = row.GetCell(4)?.ToString()?.Trim();
                string? roleName = row.GetCell(5)?.ToString()?.Trim();

                if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                {
                    errors.Add($"Dòng {rowIndex + 1}: Email hoặc Password bị trống");
                    continue;
                }

                // Kiểm tra email hoặc phone tồn tại
                var emailExists = _unitOfWork.Repository<Account>().Find(a => a.Email == email);
                if (emailExists != null)
                {
                    errors.Add($"Dòng {rowIndex + 1}: Email '{email}' đã tồn tại");
                    continue;
                }
                if (!string.IsNullOrWhiteSpace(phoneNumber))
                {
                    var phoneExists = _unitOfWork.Repository<Account>().Find(a => a.PhoneNumber == phoneNumber);
                    if (phoneExists != null)
                    {
                        errors.Add($"Dòng {rowIndex + 1}: Số điện thoại '{phoneNumber}' đã tồn tại");
                        continue;
                    }
                }

                // Lấy RoleId theo roleName
                if (string.IsNullOrWhiteSpace(roleName))
                {
                    errors.Add($"Dòng {rowIndex + 1}: Role không được để trống");
                    continue;
                }

                string roleNameLower = roleName.ToLower();

                var role = await _unitOfWork.Repository<Role>()
                    .GetAll()
                    .FirstOrDefaultAsync(r => r.RoleName.ToLower() == roleNameLower);

                if (role == null)
                {
                    errors.Add($"Dòng {rowIndex + 1}: Role '{roleName}' không tồn tại");
                    continue;
                }

                CreatePasswordHash(password, out string passwordHash);

                var account = new Account
                {
                    FullName = fullName,
                    Email = email,
                    PhoneNumber = phoneNumber,
                    ClassName = string.IsNullOrWhiteSpace(className) ? null : className,
                    RoleId = role.RoleId,
                    PasswordHash = passwordHash,
                    CreatedAt = DateTime.UtcNow,
                    Status = true 
                };

                await _unitOfWork.Repository<Account>().InsertAsync(account);
                await _unitOfWork.CommitAsync();

               
                var wallet = new Wallet
                {
                    AccountId = account.AccountId,
                    Balance = 0,
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<Wallet>().InsertAsync(wallet);
                await _unitOfWork.CommitAsync();

                createdAccounts.Add(new AccountResponse
                {
                    Id = account.AccountId,
                    Email = account.Email,
                    FullName = account.FullName,
                    PhoneNumber = account.PhoneNumber,
                    ClassName = account.ClassName,   
                    RoleId = account.RoleId,
                    CreatedAt = account.CreatedAt,
                    Status = account.Status,
                    WalletId = wallet.WalletId,
                    Balance = wallet.Balance
                });

               
                var relation = new SchoolAccountRelation
                {
                    SchoolId = schoolId,
                    AccountId = account.AccountId,
                    RelationType = role.RoleName
                };
                await _unitOfWork.Repository<SchoolAccountRelation>().InsertAsync(relation);
                await _unitOfWork.CommitAsync();
            }

            return (createdAccounts, errors);
        }


        private async Task<string> GetRelationTypeForAccountAsync(int accountId)
        {
            
            return "student"; 
        }
        public async Task<bool> UpdatePasswordAsync(int accountId, string oldPassword, string newPassword)
        {
            {
                
                var account = await _unitOfWork.Repository<Account>()
                    .GetAll()
                    .FirstOrDefaultAsync(a => a.AccountId == accountId);

                if (account == null)
                    throw new Exception("❌ Tài khoản không tồn tại");

             
                bool isOldPasswordCorrect = BCrypt.Net.BCrypt.Verify(oldPassword, account.PasswordHash);
                if (!isOldPasswordCorrect)
                    throw new Exception("❌ Mật khẩu cũ không đúng");

               
                string hashedNewPassword = BCrypt.Net.BCrypt.HashPassword(newPassword);

                
                account.PasswordHash = hashedNewPassword;
                await _unitOfWork.SaveChangesAsync();

                return true;
            }
        }
        public async Task SendEmailWithButtonAsync(string toEmail)
        {
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"]);
            var user = smtpSection["User"];
            var pass = smtpSection["Pass"];
            var enableSsl = bool.Parse(smtpSection["EnableSsl"]);

            var defaultLink = _config["Smtp:DefaultLink"] ?? "https://google.com";

            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Email == toEmail);

            if (account == null)
                throw new Exception($"Account with email {toEmail} not found");

          
            var uriBuilder = new UriBuilder(defaultLink);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            query["id"] = account.AccountId.ToString();           
            uriBuilder.Query = query.ToString();
            var buttonLink = uriBuilder.ToString();
            var htmlBody = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta http-equiv='Content-Type' content='text/html charset=UTF-8' />
</head>
<body style='background-color:#fff;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen-Sans,Ubuntu,Cantarell,Helvetica Neue,sans-serif'>
    <div style='width:50vw; margin: 0 auto'>
        <div style='width: 100%; height: 200px; margin: 0 auto;'>
            <img src='https://live.staticflickr.com/65535/54733176285_f474a3aaed_n.jpg'
                style='width: auto;height:200px;object-fit: cover; margin-left: 35%;'>
        </div>
        <table style='padding:0 40px' align='center' border='0' cellPadding='0' cellSpacing='0' role='presentation' width='100%'>
            <tbody>
                <tr>
                    <td>
                        <hr style='width:100%;border:none;border-top:1px solid black;margin:20px 0' />
                        <p style='font-size:14px;line-height:22px;margin:16px 0;color:#3c4043;margin-bottom: 25px;'>
                            Kính chào <a style='font-size:16px;font-weight: bold;'>USER_NAME</a>,
                        </p>
                        <p style='font-size:14px;line-height:22px;margin:16px 0;color:#3c4043;text-align: justify'>
                            Chúng tôi xin thông báo rằng tài khoản của Quý Trường đã được khởi tạo trên hệ thống. Thông tin cụ thể như sau:
                        </p>
                        <div style='margin-left: 25px;'>
                            <p style='font-size:14px;margin:10px 0;color:#3c4043'>Email đăng nhập:
                                <a style='font-weight:bold;text-decoration:none;'>[user_email]</a>
                            </p>
                            <p style='font-size:14px;margin:10px 0;color:#3c4043'>Mật khẩu tạm thời:
                                <a style='font-weight:bold;text-decoration:none;'>[user_password]</a>
                            </p>
                        </div>
                        <p style='font-size:14px;margin:16px 0;color:#3c4043;text-align: justify'>
                            Để hoàn tất việc kích hoạt và đăng nhập, vui lòng bấm vào nút dưới đây:
                        </p>
                        <div>
                            <a href='BỎ LINK VÀO ĐÂY' style='display:inline-block;padding:12px 24px;background-color:#007BFF;
                               color:#ffffff;font-weight:bold;font-size:14px;text-decoration:none;border-radius:6px;'>
                                Bấm vào đây để xác nhận
                            </a>
                        </div>
                        <p style='font-size:14px;margin:16px 0;color:#3c4043;text-align: justify'>
                            Sau khi đăng nhập lần đầu, vui lòng đổi mật khẩu để đảm bảo an toàn thông tin.
                        </p>
                        <p style='font-size:14px;margin:16px 0;color:#3c4043;text-align: justify'>
                            Nếu có bất kỳ thắc mắc hoặc cần hỗ trợ, xin vui lòng liên hệ với chúng tôi qua địa chỉ email này.
                        </p>
                        <p style='font-size:14px;margin:16px 0;color:#3c4043'>Trân trọng,</p>
                        <p style='font-weight:bold;font-size:16px;margin:16px 0 0 0;color:#3c4043'>Festival Hub</p>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</body>
</html>";


            htmlBody = htmlBody.Replace("USER_NAME", account.FullName)
                       .Replace("[user_email]", account.Email)
                       .Replace("[user_password]", account.PlainPassword ?? "Mật khẩu đã xoá")
                       .Replace("BỎ LINK VÀO ĐÂY", buttonLink);

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };

            using var mail = new MailMessage(user, toEmail, "Thông tin tài khoản của bạn", htmlBody)
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mail);
            account.PlainPassword = null;
            account.Status = true;
            account.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();
        }




        public async Task<Account> UpdateSchoolManagerStatusAsync(int accountId, bool? status = true)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .Include(a => a.Role)
                .FirstOrDefaultAsync(a => a.AccountId == accountId);

            if (account == null)
                throw new CrudException(HttpStatusCode.NotFound, "Account không tồn tại", accountId.ToString());

           
            if (account.Role == null || account.Role.RoleName != Roles.SchoolManager)
                throw new CrudException(HttpStatusCode.BadRequest, "Chỉ account có role SchoolManager mới được cập nhật status", accountId.ToString());

            
            account.Status = status ?? true;
            account.UpdatedAt = DateTime.UtcNow;


            await _unitOfWork.CommitAsync();

            return account;
        }



        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 6) 
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


        public async Task<bool> ForgotPasswordAsync(string email)
        {
           
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản với email này", email);

            
            var newPassword = GenerateRandomPassword();

           
            CreatePasswordHash(newPassword, out string passwordHash);

          
            account.PasswordHash = passwordHash;
            account.PlainPassword = newPassword;
            account.UpdatedAt = DateTime.UtcNow;


            await _unitOfWork.CommitAsync();

           
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"]);
            var user = smtpSection["User"];
            var pass = smtpSection["Pass"];
            var enableSsl = bool.Parse(smtpSection["EnableSsl"]);

           
            var htmlBody = @"
<!DOCTYPE html>
<html lang='en'>
<head>
    <meta http-equiv='Content-Type' content='text/html charset=UTF-8' />
</head>
<body style='background-color:#fff;font-family:-apple-system,BlinkMacSystemFont,Segoe UI,Roboto,Oxygen-Sans,Ubuntu,Cantarell,Helvetica Neue,sans-serif'>
    <div style='width:50vw; margin: 0 auto'>
        <div style='width: 100%; height: 200px; margin: 0 auto;'>
            <img src='https://live.staticflickr.com/65535/54733176285_f474a3aaed_n.jpg'
                style='width: auto;height:200px;object-fit: cover; margin-left: 35%;'>
        </div>
        <table style='padding:0 40px' align='center' border='0' cellPadding='0' cellSpacing='0' role='presentation' width='100%'>
            <tbody>
                <tr>
                    <td>
                        <hr style='width:100%;border:none;border-top:1px solid black;margin:20px 0' />
                        <p style='font-size:14px;line-height:22px;margin:16px 0;color:#3c4043;margin-bottom: 25px;'>
                            Kính chào <a style='font-size:16px;font-weight: bold;'>" + account.FullName + @"</a>,
                        </p>
                        <p style='font-size:14px;line-height:22px;margin:16px 0;color:#3c4043;text-align: justify'>
                            Bạn vừa yêu cầu <b>khôi phục mật khẩu</b>. Thông tin đăng nhập mới của bạn như sau:
                        </p>
                        <div style='margin-left: 25px;'>
                            <p style='font-size:14px;margin:10px 0;color:#3c4043'>Email đăng nhập:
                                <a style='font-weight:bold;text-decoration:none;'>" + account.Email + @"</a>
                            </p>
                            <p style='font-size:14px;margin:10px 0;color:#3c4043'>Mật khẩu mới:
                                <a style='font-weight:bold;text-decoration:none;'>" + newPassword + @"</a>
                            </p>
                        </div>
                        <p style='font-size:14px;margin:16px 0;color:#3c4043;text-align: justify'>
                            Vui lòng đăng nhập và <b>đổi mật khẩu ngay sau khi truy cập</b> để đảm bảo an toàn thông tin.
                        </p>
                        <p style='font-size:14px;margin:16px 0;color:#3c4043'>Trân trọng,</p>
                        <p style='font-weight:bold;font-size:16px;margin:16px 0 0 0;color:#3c4043'>Festival Hub</p>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</body>
</html>";

           
            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };

            using var mail = new MailMessage(user, account.Email, "Khôi phục mật khẩu", htmlBody)
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mail);

            return true;
        }


        private string GenerateOtp()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString(); 
        }

        public async Task<bool> SendOtpAsync(string email)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", email);

            var otp = GenerateOtp();

           
            account.OtpVerify = otp;
            account.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            
            var smtpSection = _config.GetSection("Smtp");
            var host = smtpSection["Host"];
            var port = int.Parse(smtpSection["Port"]);
            var user = smtpSection["User"];
            var pass = smtpSection["Pass"];
            var enableSsl = bool.Parse(smtpSection["EnableSsl"]);

           
            var htmlBody = @"
<!DOCTYPE html
    PUBLIC '-//W3C//DTD XHTML 1.0 Transitional//EN' 'http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd'>
<meta http-equiv='Content-Type' content='text/html charset=UTF-8' />
<html lang='en'>

<body style='background-color:#ffffff;font-family:HelveticaNeue,Helvetica,Arial,sans-serif'>
    <table align='center' role='presentation' cellSpacing='0' cellPadding='0' border='0' width='100%'
        style='max-width:37.5em;background-color:#ffffff;border:1px solid #eee;border-radius:5px;
               box-shadow:0 5px 10px rgba(20,50,70,.2);margin-top:20px;width:360px;margin:0 auto;padding:68px 0 68px'>
        <tr style='width:100%'>
            <td>
                <img alt='FestivalPlatform' src='https://live.staticflickr.com/65535/54733176285_f474a3aaed_n.jpg'
                    width='200' height='auto'
                    style='display:block;outline:none;border:none;text-decoration:none;margin:0 auto' />

                <p style='font-size:11px;line-height:16px;margin:16px 8px 8px 8px;color:#0a85ea;font-weight:700;
                          font-family:HelveticaNeue,Helvetica,Arial,sans-serif;height:16px;text-align:center'>
                    Xác thực Email
                </p>

                <h1 style='color:#000;font-family:HelveticaNeue-Medium,Helvetica,Arial,sans-serif;font-size:20px;
                           font-weight:500;line-height:24px;margin:0;text-align:center'>
                    Xin chào <b>" + account.FullName + @"</b>, đây là mã OTP để hoàn thành việc xác thực email của bạn
                </h1>

                <table style='background:rgba(0,0,0,.05);border-radius:4px;margin:16px auto 14px;
                               vertical-align:middle;width:280px' align='center'>
                    <tbody>
                        <tr>
                            <td>
                                <p style='font-size:32px;line-height:40px;margin:0 auto;color:#000;
                                          font-family:HelveticaNeue-Bold;font-weight:700;letter-spacing:6px;
                                          padding:8px 0;text-align:center'>
                                    " + otp + @"
                                </p>
                            </td>
                        </tr>
                    </tbody>
                </table>

                <p style='font-size:15px;line-height:23px;margin:0;color:#444;font-family:HelveticaNeue,Helvetica,Arial,sans-serif;
                          padding:0 40px;text-align:center'>
                    Liên hệ <a target='_blank' style='color:#444;text-decoration:underline'
                        href='mailto:festivalplatform@gmail.com'>festivalplatform@gmail.com</a> 
                    nếu bạn không yêu cầu chuyện này!
                </p>
            </td>
        </tr>
    </table>
</body>
</html>";

            using var smtp = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(user, pass),
                EnableSsl = enableSsl
            };

            using var mail = new MailMessage(user, account.Email, "Xác thực OTP", htmlBody)
            {
                IsBodyHtml = true
            };

            await smtp.SendMailAsync(mail);
            return true;
        }
        public async Task<bool> ConfirmOtpAsync(string email, string otp)
        {
            var account = await _unitOfWork.Repository<Account>()
                .GetAll()
                .FirstOrDefaultAsync(a => a.Email == email);

            if (account == null)
                throw new CrudException(HttpStatusCode.NotFound, "Không tìm thấy tài khoản", email);

            if (account.OtpVerify == null || account.OtpVerify != otp)
                throw new CrudException(HttpStatusCode.BadRequest, "OTP không hợp lệ", otp);

            
            account.OtpVerify = null;
            account.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.CommitAsync();

            
            return await ForgotPasswordAsync(email);
        }

    }

}
