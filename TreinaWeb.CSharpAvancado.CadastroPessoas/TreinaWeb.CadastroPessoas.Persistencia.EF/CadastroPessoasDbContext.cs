using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TreinaWeb.CadastroPessoas.Dominio;

namespace TreinaWeb.CadastroPessoas.Persistencia.EF
{
    //DbContext é o banco de dados para o Entity Framework
    public class CadastroPessoasDbContext : DbContext
    {
        //DbSet é a tabela do banco de dados
        public DbSet<Pessoa> Pessoas { get; set; }

        /*
         * Comandos do Entity Framework (feitos pelo PAckage Manager Console)
         * 
         * Habilitando as Migrations no projeto
         * Enable-Migrations -ProjectName TreinaWeb.CadastroPessoas.Persistencia.EF -StartUpProjectName TreinaWeb.CadastroPessoas.Persistencia.EF
         * 
         * Criando uma Migration
         * Add-Migration "Criacao_Tabela_Pessoas" -ProjectName TreinaWeb.CadastroPessoas.Persistencia.EF -StartUpProjectName TreinaWeb.CadastroPessoas.Persistencia.EF
         * 
         * Atulizar o banco de dados
         * Update-Database -ProjectName TreinaWeb.CadastroPessoas.Persistencia.EF -StartUpProjectName TreinaWeb.CadastroPessoas.Persistencia.EF -Verbose
         */
    }
}
