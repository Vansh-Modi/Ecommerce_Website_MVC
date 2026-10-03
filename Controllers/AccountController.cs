using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
using Ecommerce_Website_MVC.Helpers;

namespace Ecommerce_Website_MVC.Controllers
{
    public class AccountController : Controller
    {
        private string ConnStr => ConfigurationManager.ConnectionStrings["ShowsGarage"].ConnectionString;

        [HttpGet]
        public ActionResult Login(string returnUrl = null)
        {
            if (Session["UserRole"] != null)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string email, string password, string returnUrl = null)
        {
            try
            {
                const string q = "SELECT UserID, Role, FullName, Email FROM Users WHERE Email=@email AND PasswordHash=@pass";
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@email", email.Trim());
                    cmd.Parameters.AddWithValue("@pass", password.Trim());
                    conn.Open();

                    using (var dr = cmd.ExecuteReader())
                    {
                        if (dr.Read())
                        {
                            Session["UserID"]    = dr["UserID"].ToString();
                            Session["UserRole"]  = dr["Role"].ToString();
                            Session["UserName"]  = dr["FullName"].ToString();
                            Session["UserEmail"] = dr["Email"].ToString();

                            if (dr["Role"].ToString() == "Admin")
                                return RedirectToAction("Dashboard", "Admin");

                            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                                return Redirect(returnUrl);

                            return RedirectToAction("Index", "Home");
                        }
                    }
                }
                ViewBag.Error = "Invalid email or password. Please try again.";
            }
            catch (Exception ex)
            {
                ViewBag.Error = "System Error: " + ex.Message;
            }

            ViewBag.ReturnUrl = returnUrl;
            return View();
        }


        [HttpGet]
        public ActionResult ForgotPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(string forgotEmail)
        {
            try
            {
                const string q = "SELECT COUNT(1) FROM Users WHERE Email = @email";
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@email", forgotEmail.Trim());
                    conn.Open();
                    int exists = Convert.ToInt32(cmd.ExecuteScalar());

                    if (exists > 0)
                    {
                        string otp = new Random().Next(100000, 999999).ToString();
                        Session["RecoveryOTP"]         = otp;
                        Session["RecoveryTargetEmail"] = forgotEmail.Trim();

                        EmailHelper.SendHtml(
                            forgotEmail.Trim(),
                            "Security Recovery Account Code - Show's Garage",
                            EmailHelper.BuildPasswordResetEmail(otp));

                        TempData["ForgotSuccess"] = "OTP dispatched to your email.";
                        return RedirectToAction("ResetPassword");
                    }

                    ViewBag.Error = "This email address is not registered.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "SMTP Error: " + ex.Message;
            }
            return View();
        }


        [HttpGet]
        public ActionResult ResetPassword() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(string otp, string newPass)
        {
            if (Session["RecoveryOTP"] == null || otp.Trim() != Session["RecoveryOTP"].ToString())
            {
                ViewBag.Error = "The verification OTP code is incorrect.";
                return View();
            }

            string targetEmail = Session["RecoveryTargetEmail"].ToString();

            try
            {
                using (var conn = new SqlConnection(ConnStr))
                {
                    conn.Open();

                    string currentHash = string.Empty;
                    using (var cmd = new SqlCommand("SELECT PasswordHash FROM Users WHERE Email = @email", conn))
                    {
                        cmd.Parameters.AddWithValue("@email", targetEmail);
                        var res = cmd.ExecuteScalar();
                        if (res != null) currentHash = res.ToString();
                    }

                    if (newPass.Trim() == currentHash)
                    {
                        ViewBag.Error = "You cannot reuse your current password.";
                        return View();
                    }

                    using (var cmd = new SqlCommand("UPDATE Users SET PasswordHash = @p WHERE Email = @e", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", newPass.Trim());
                        cmd.Parameters.AddWithValue("@e", targetEmail);
                        cmd.ExecuteNonQuery();
                    }
                }

                Session["RecoveryOTP"]         = null;
                Session["RecoveryTargetEmail"] = null;
                TempData["LoginSuccess"]        = "Password updated! You can now log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Save Failure: " + ex.Message;
                return View();
            }
        }


        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendOtp(string name, string phone, string email, string password)
        {
            if (string.IsNullOrEmpty(email))
            {
                ViewBag.Error = "Please enter an email address first.";
                return View("Register");
            }

            Session["Reg_Name"]     = name;
            Session["Reg_Phone"]    = phone;
            Session["Reg_Email"]    = email;
            Session["Reg_Password"] = password;

            string otp = new Random().Next(100000, 999999).ToString();
            Session["GeneratedOTP"] = otp;

            try
            {
                EmailHelper.SendHtml(email.Trim(),
                    "Your Verification Security Code",
                    EmailHelper.BuildOtpEmail(otp));

                TempData["OtpSent"] = "OTP sent to " + email.Trim() + ". Check Spam/Junk if not found.";
                return RedirectToAction("VerifyOtp");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Mail Delivery Error: " + ex.Message;
                return View("Register");
            }
        }

        [HttpGet]
        public ActionResult VerifyOtp()
        {
            if (Session["GeneratedOTP"] == null)
                return RedirectToAction("Register");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult VerifyOtp(string otp)
        {
            if (Session["GeneratedOTP"] == null || otp.Trim() != Session["GeneratedOTP"].ToString())
            {
                ViewBag.Error = "Invalid OTP. Please check your verification code and try again.";
                return View();
            }

            Session["GeneratedOTP"] = null;

            try
            {
                const string q = "INSERT INTO Users (FullName, Phone, Email, PasswordHash, Role, IsVerified) VALUES (@name, @phone, @email, @pass, @role, @verified)";
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand(q, conn))
                {
                    cmd.Parameters.AddWithValue("@name",     Session["Reg_Name"].ToString());
                    cmd.Parameters.AddWithValue("@phone",    Session["Reg_Phone"].ToString());
                    cmd.Parameters.AddWithValue("@email",    Session["Reg_Email"].ToString());
                    cmd.Parameters.AddWithValue("@pass",     Session["Reg_Password"].ToString());
                    cmd.Parameters.AddWithValue("@role",     "Client");
                    cmd.Parameters.AddWithValue("@verified", true);
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }

                Session["Reg_Name"] = Session["Reg_Phone"] = Session["Reg_Email"] = Session["Reg_Password"] = null;
                TempData["LoginSuccess"] = "Registration successful! You can now log in.";
                return RedirectToAction("Login");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;
                return View();
            }
        }


        [HttpGet]
        public ActionResult Profile()
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("Login", new { returnUrl = Request.Url.PathAndQuery });

            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("SELECT FullName, Phone, Email FROM Users WHERE Email = @email", conn))
                {
                    cmd.Parameters.AddWithValue("@email", Session["UserEmail"].ToString());
                    conn.Open();
                    using (var rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            ViewBag.FullName = rdr["FullName"].ToString();
                            ViewBag.Phone    = rdr["Phone"].ToString();
                            ViewBag.Email    = rdr["Email"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Error loading profile: " + ex.Message;
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("UpdateProfile")]
        public ActionResult UpdateProfile(string fullName, string phone)
        {
            if (Session["UserRole"] == null)
                return RedirectToAction("Login");

            try
            {
                using (var conn = new SqlConnection(ConnStr))
                using (var cmd = new SqlCommand("UPDATE Users SET FullName=@n, Phone=@p WHERE Email=@e", conn))
                {
                    cmd.Parameters.AddWithValue("@n", fullName.Trim());
                    cmd.Parameters.AddWithValue("@p", phone.Trim());
                    cmd.Parameters.AddWithValue("@e", Session["UserEmail"].ToString());
                    conn.Open();
                    cmd.ExecuteNonQuery();
                }
                Session["UserName"] = fullName.Trim();
                TempData["Success"] = "Profile updated successfully!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Profile Save Error: " + ex.Message;
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SendProfileOtp()
        {
            if (Session["UserRole"] == null) return RedirectToAction("Login");

            string email = Session["UserEmail"].ToString();
            string otp   = new Random().Next(100000, 999999).ToString();
            Session["ProfileRecoveryOTP"] = otp;

            try
            {
                EmailHelper.SendHtml(email, "Profile Security Credentials Token - Show's Garage",
                    EmailHelper.BuildProfileOtpEmail(otp));
                TempData["OtpSent"] = "A security token has been sent to your inbox.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "SMTP Error: " + ex.Message;
            }
            return RedirectToAction("Profile");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdatePassword(string currentPassword, string profileOtp, string newPassword, string confirmNewPassword, bool useOtp = false)
        {
            if (Session["UserRole"] == null) return RedirectToAction("Login");

            if (newPassword != confirmNewPassword)
            {
                TempData["Error"] = "New passwords do not match.";
                return RedirectToAction("Profile");
            }

            string targetEmail = Session["UserEmail"].ToString();
            try
            {
                using (var conn = new SqlConnection(ConnStr))
                {
                    conn.Open();
                    string currentHash = string.Empty;
                    using (var cmd = new SqlCommand("SELECT PasswordHash FROM Users WHERE Email=@e", conn))
                    {
                        cmd.Parameters.AddWithValue("@e", targetEmail);
                        var r = cmd.ExecuteScalar();
                        if (r != null) currentHash = r.ToString();
                    }

                    if (useOtp)
                    {
                        if (Session["ProfileRecoveryOTP"] == null || profileOtp.Trim() != Session["ProfileRecoveryOTP"].ToString())
                        {
                            TempData["Error"] = "The verification OTP is incorrect.";
                            return RedirectToAction("Profile");
                        }
                    }
                    else if (currentPassword != currentHash)
                    {
                        TempData["Error"] = "Incorrect current password.";
                        return RedirectToAction("Profile");
                    }

                    if (newPassword.Trim() == currentHash)
                    {
                        TempData["Error"] = "You cannot reuse your current password.";
                        return RedirectToAction("Profile");
                    }

                    using (var cmd = new SqlCommand("UPDATE Users SET PasswordHash=@p WHERE Email=@e", conn))
                    {
                        cmd.Parameters.AddWithValue("@p", newPassword.Trim());
                        cmd.Parameters.AddWithValue("@e", targetEmail);
                        cmd.ExecuteNonQuery();
                    }

                    Session["ProfileRecoveryOTP"] = null;
                    TempData["Success"] = "Password updated successfully!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Credentials Update Failure: " + ex.Message;
            }
            return RedirectToAction("Profile");
        }


        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }
    }
}