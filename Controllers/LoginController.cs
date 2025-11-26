using System;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Security;
using Org.BouncyCastle.Tls;
using WEBSGI.Models;
using WEBSGI.Models.ViewModel;
using WEBSGI.Repositorio;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.StartPanel;

namespace WEBSGI.Controllers
{

    [RequireHttps]

    public class LoginController : Controller
    {
        private readonly RepositorioUsuarios _repositorioUsuario;
        private readonly RepositorioLog _repositorioLog;

        // Constructor sin parámetros requerido por MVC
        public LoginController()
        {
            _repositorioLog = new RepositorioLog();
            _repositorioUsuario = new RepositorioUsuarios(); // Crear instancia manualmente
        }

        // Constructor con inyección de dependencias
        public LoginController(RepositorioUsuarios repositorioUsuario, RepositorioLog repositorioLog)
        {
            _repositorioUsuario = repositorioUsuario;
            _repositorioLog = repositorioLog;
        }

        [HttpGet]
        public ActionResult Login()
        {


            return View(new LoginViewModel());
        }

       [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Datos de inicio de sesión inválidos.";
                TempData["AlertType"] = "error"; // Tipo de alerta
                return RedirectToAction("Login");  // Redirigimos a la misma vista
            }

            if (!Request.IsLocal && !Request.IsSecureConnection)
            {
                string redirectUrl = Request.Url.ToString().Replace("http", "https");
                TempData["ErrorMessage"] = "Redirigiendo a una conexión segura.";
                TempData["AlertType"] = "warning"; // Tipo de alerta
                TempData["RedirectUrl"] = redirectUrl;
                return RedirectToAction("Login");  // Redirigimos a la misma vista
            }

            if (model != null)
            {
                Usuarios usuario = _repositorioUsuario.Login(model.Username, model.Password);
                if (usuario != null)
                {
                    string contrasenaVieja = model.Password;
                    Session["ContrasenaVieja"] = contrasenaVieja;

                    TempData["Usuario"] = usuario.Usuario;

                    Session["USUARIO"] = usuario.Usuario;
                    Session["numero"] = usuario.Usuario;
                    int DAIS = DateTime.Now.Date.Subtract(usuario.FechaActualizaClave).Days;
                    //DAIS = DAIS + 22;

                    if (DAIS > 20)
                    {
                        usuario.NecesitaCambiarContrasena = true;
                        
                      
                        TempData["AlertMessage"] = "Por favor, actualice su clave para continuar.";
                        TempData["MostrarModalCambio"] = "true"; // Controlamos la apertura del modal

                        Session["Usuario"] = usuario;
                        Session["numero"] = usuario.Numero;

                        return RedirectToAction("Login");
                    }
                    else
                    {
                        Session["Usuario"] = usuario;
                        Session["idUsuario"] = usuario.IdUsuario;
                        Session["NOMBREUSUARIO"] = usuario.Nombre;
                        Session["NUMERO"] = usuario.Numero;
                        Session["ROL"] = usuario.Nivel;
                        Session["PUESTO"] = usuario.Puesto;

                        string puestoStr = Session["PUESTO"] as string;
                        Session["PUESTOID"] = _repositorioUsuario.TraerPuestoPorNombre(puestoStr);
                        Session["NIVEL"] = usuario.Nivel;
                        Session["FECHAMODCLAVE"] = usuario.FechaActualizaClave;

                        //_repositorioLog.ControlUsuario(usuario);

                        string IPPRIVADA = Request.ServerVariables["REMOTE_HOST"];
                        string ipPublica = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                        if (string.IsNullOrEmpty(ipPublica))
                        {
                            ipPublica = Request.ServerVariables["REMOTE_ADDR"];
                        }

                        Session["IPPUBLICA"] = ipPublica;
                        Session["IPPRIVADA"] = IPPRIVADA;

                        Log LL = new Log
                        {
                            USUARIO = usuario.Usuario,
                            ARCHIVO_MOVIMIENTO = usuario.Nombre +", Inició sesión",
                            IPPUBLICA = ipPublica,
                            IPPRIVADA = IPPRIVADA,
                            FECHA = DateTime.Now
                        };

                        _repositorioLog.Agregar(LL);

                        TempData["SuccessMessage"] = "Bienvenido, su sesión ha sido iniciada correctamente.";
                        TempData["AlertType"] = "success"; // Alerta de éxito
                        return RedirectToAction("Index", "Inicio");
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Usuario o clave incorrectos.";
                    TempData["AlertType"] = "error"; // Tipo de alerta
                    return RedirectToAction("Login");  // Redirigimos a la misma vista
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Ocurrió un error inesperado.";
                TempData["AlertType"] = "error"; // Tipo de alerta
                return RedirectToAction("Login");
            }
        }



        [HttpPost]
        public ActionResult GenerarNuevaContraseña(string nuevaContraseña, string confirmarContraseña)
        {


            // Validaciones
            if (nuevaContraseña != confirmarContraseña || nuevaContraseña.Length <= 6)
            {
                return Json(new { success = false, message = "Las contraseñas deben ser iguales y mayores a 6 caracteres." });
            }
            if (nuevaContraseña == Session["ContrasenaVieja"].ToString())
            {
                return Json(new { success = false, message = "La nueva contraseña no puede ser igual a la anterior." });
            }

            if (!_repositorioUsuario.ContrasenaSegura(nuevaContraseña))
            {
                return Json(new { success = false, message = "La contraseña no cumple con los requisitos de seguridad." });
            }

            // Cambio de contraseña
            int usuarioNumero = int.Parse(Session["numero"].ToString());
            _repositorioUsuario.ModificarContraseña(usuarioNumero, nuevaContraseña);

            // Actualizar usuario en sesión
            Usuarios usuario = _repositorioUsuario.Login(Session["numero"].ToString(), nuevaContraseña);

            Session["Usuario"] = usuario;
    

            Session["idUsuario"] = usuario.IdUsuario;
            Session["ROL"] = usuario.Nivel;
            Session["PUESTO"] = usuario.Puesto;
            Session["NOMBREUSUARIO"] = usuario.Nombre;
            Session["NUMERO"] = usuario.Numero;

            string puestoStr = Session["PUESTO"] as string;
            Session["PUESTOID"] = _repositorioUsuario.TraerPuestoPorNombre(puestoStr);
            Session["NIVEL"] = usuario.Nivel;
            Session["FECHAMODCLAVE"] = usuario.FechaActualizaClave;

            //_repositorioLog.ControlUsuario(usuario);

            string IPPRIVADA = Request.ServerVariables["REMOTE_HOST"];
            string IPPUBLICA = Request.ServerVariables["REMOTE_ADDR"];
            Session["IPPUBLICA"] = IPPUBLICA;
            Session["IPPRIVADA"] = IPPRIVADA;

            Log LL = new Log
            {
                USUARIO = usuario.Usuario,
                ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Inició sesion y ademas cambió su contraseña",
                IPPUBLICA = IPPUBLICA,
                IPPRIVADA = IPPRIVADA,
                FECHA = DateTime.Now
            };

            _repositorioLog.Agregar(LL);

            // Retornar JSON de éxito
            return Json(new { success = true, message = "Contraseña actualizada correctamente." });
        }


        public ActionResult Logout()
        {
            Log LL = new Log();
            int numero = Convert.ToInt32(Session["NUMERO"]);
            Usuarios usuario = _repositorioLog.BuscarUNO(numero);
            LL.USUARIO = usuario.Usuario;
            LL.ARCHIVO_MOVIMIENTO = usuario.Nombre + ", Cerró sesion" ;
            LL.FECHA = DateTime.Now;
            LL.IPPUBLICA = Session["IPPUBLICA"].ToString();
            _repositorioLog.Agregar(LL);

            FormsAuthentication.SignOut();
            Session.Abandon();
            return RedirectToAction("Login", "Login");
        }

        [HttpGet]
        public ActionResult EnviarCorreoRecuperacion(string numero)
        {
            bool exito = _repositorioUsuario.EnviarCorreoRecuperacion(numero);
            string mensaje = exito ? "¡Hola! Por favor, revisa tu casilla de correo electrónico. Si no ves el mensaje en tu bandeja de entrada, verifica la carpeta de spam o correo no deseado." : "Usuario no encontrado.";

            // Regresar un JSON con el resultado de la operación
            return Json(new { success = exito, message = mensaje }, JsonRequestBehavior.AllowGet);
        }







    }







}





