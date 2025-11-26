using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBSGI.Models
{
    public class Usuarios
    {
        public int IdUsuario { get; set; }
        public string Nombre { get; set; }
        public string Usuario { get; set; }
        public string Contrasena { get; set; }
        public int Numero { get; set; }

        public DateTime FechaNacimiento { get; set; }
        public DateTime VtoCarnetSalud { get; set; }
        public DateTime VtoPorteArma { get; set; }
        public DateTime VtoLibreta { get; set; }

        public string Uniforme { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public int Olvido { get; set; }

        public int AdelantosFijos { get; set; }
        public int Cedula { get; set; }
        public string Estado { get; set; }

        public int Aviso { get; set; }
        public string Email { get; set; }

        public string Sector { get; set; }
        public string Rol { get; set; }
        public DateTime FechaActualizaClave { get; set; }
  
        public string Nivel { get; set; }

        public string Activo { get; set; }
        public string Puesto { get; set; }

        public bool NecesitaCambiarContrasena { get; set; }
        public Usuarios() { }


        public Usuarios(
    int idUsuario,
    string nombre,
    string usuario,
    string contrasena,
    int numero,
    DateTime fechaNacimiento,
    DateTime vtoCarnetSalud,
    DateTime vtoPorteArma,
    DateTime vtoLibreta,
    string uniforme,
    string direccion,
    string telefono,
    int olvido,
    int adelantosFijos,
    int cedula,
    string estado,
    int aviso,
    string email,
    string sector,
    string rol,
    DateTime fechaActualizaClave,
    string nivel,
    string activo,
    string puesto,
    bool necesitaCambiarContrasena)
        {
            this.IdUsuario = idUsuario;
            this.Nombre = nombre;
            this.Usuario = usuario;
            this.Contrasena = contrasena;
            this.Numero = numero;
            this.FechaNacimiento = fechaNacimiento;
            this.VtoCarnetSalud = vtoCarnetSalud;
            this.VtoPorteArma = vtoPorteArma;
            this.VtoLibreta = vtoLibreta;
            this.Uniforme = uniforme;
            this.Direccion = direccion;
            this.Telefono = telefono;
            this.Olvido = olvido;
            this.AdelantosFijos = adelantosFijos;
            this.Cedula = cedula;
            this.Estado = estado;
            this.Aviso = aviso;
            this.Email = email;
            this.Sector = sector;
            this.Rol = rol;
            this.FechaActualizaClave = fechaActualizaClave;
            this.Nivel = nivel;
            this.Activo = activo;
            this.Puesto = puesto;
            this.NecesitaCambiarContrasena = necesitaCambiarContrasena;
        }

    }
}