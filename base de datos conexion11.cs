using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

public class ConexionBD
{
  
    private static string cadena = "Server=DESKTOP-P89RT7D\\SQLEXPRESS;Database=ZazonApp;Trusted_Connection=True;";

    public static SqlConnection ObtenerConexion()
    {
        return new SqlConnection(cadena);
    }
}
