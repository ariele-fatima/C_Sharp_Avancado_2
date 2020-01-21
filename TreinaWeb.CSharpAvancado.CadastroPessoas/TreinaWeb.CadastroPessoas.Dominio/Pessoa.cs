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
        public virtual int Id { get; set; }
        public virtual string Nome { get; set; }
        public virtual int Idade { get; set; }
        public virtual string Endereco { get; set; }
    }
}
