using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//Camada do negócio
namespace TreinaWeb.CadastroPessoas.Dominio
{
    //Classe POCO - Plain Old CLR Object, uma classe pura, simples, como apenas propriedades 
    public class Pessoa
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public int Idade { get; set; }
        public string Endereco { get; set; }
    }
}
