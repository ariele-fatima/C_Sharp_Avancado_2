using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreinaWeb.CadastroPessoas.Dominio;
using TreinaWeb.CadastroPessoas.Repositorio;

namespace TreinaWeb.CSharpAvancado.CadastroPessoas
{
    public partial class FrmAdicionarPessoa : Form
    {
        public FrmAdicionarPessoa()
        {
            InitializeComponent();
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            Pessoa pessoa = new Pessoa
            {
                Nome = txbNome.Text,
                Idade = Convert.ToInt32(txbIdade.Text),
                Endereco = txbEndereco.Text            
            };
            IRepositorio<Pessoa> repositorioPessoa = new PessoaRepositorio();
            //repositorioPessoa.Adicionar(pessoa);
            repositorioPessoa.AdicionarAsync(pessoa, (linhasAfetadas) => 
            {
                MessageBox.Show(string.Format("Foi inserido(s) {0} registro(s).", linhasAfetadas));
            });
            Close();
        }
    }
}
