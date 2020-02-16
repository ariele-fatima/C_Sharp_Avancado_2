using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TreinaWeb.CadastroPessoas.Dominio;
using TreinaWeb.CadastroPessoas.Repositorio;

namespace TreinaWeb.CSharpAvancado.CadastroPessoas
{
    public partial class FrmPessoas : Form
    {
        private List<Pessoa> _pessoas = new List<Pessoa>();
        //para usar a keyword lock é preciso passar de parametro um objeto privado estatico de apenas leitura
        //este servira como um semafaro para controlar o trafego de acesso ao objeto compartilhado
        private static readonly object locker = new Object();

        public FrmPessoas()
        {
            //para o form ser criado, ele utliza uma thread
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //criando uma thread para o PreencherDataGridView
            //Thread thread1 = new Thread(PreencherDataGridView); 
            //thread1.Start();

            //criando uma Task, usamos o metodo Run para executa-la e o Run espera como parametro
            //um delegate do tipo Action (tipo Action não tem retorno), que pode ser escrito com lambda
            Task.Run(() => {
                Thread.Sleep(5000);
                IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
                dgvPessoas.Invoke((MethodInvoker)delegate { 
                    dgvPessoas.DataSource = repositorioPessoas.SelecionarTodos();
                    dgvPessoas.Refresh();
                });
            });
        }

        private void PreencherDataGridView()
        {
            //Para exceções não adiantaria colocar um try catch aqui no metodo PreencherDataGridView, 
            //exceções devem ser tratadas dentro do metodo utlizado pelas threads

            Thread.Sleep(5000);
            Thread thread2 = new Thread(PreencherListaPessoas);
            Thread thread3 = new Thread(PreencherListaPessoas2);
            thread2.Start();
            thread3.Start();
            //Join é usado para aguardar a finalização de uma thread, sem ele o Invoke chama o DataGridView
            //antes da thread conseguir preencher o DataGridView com a lista de pessoas do banco de dados
            thread2.Join();
            thread3.Join();

            //como foi a thread do InitializeComponent que criou o form e seus componentes (dgvPessoas)
            //a thread que criamos não pode alterar diretamente esses componentes, por isso o uso do Invoke
            dgvPessoas.Invoke((MethodInvoker)delegate 
            {
                dgvPessoas.DataSource = _pessoas;
                dgvPessoas.Refresh();
            });
            
        }

        private void PreencherListaPessoas()
        {
            IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
            List<Pessoa> pessoas = repositorioPessoas.SelecionarTodos();
            //Use a instrução lock para sincronizar o acesso de thread com um recurso compartilhado
            lock (locker) //impede que ambas as thread manipulem o mesmo objeto (_pessoas) ao mesmo tempo
            {
                foreach (Pessoa p in pessoas)
                {
                    p.Nome += " Thread 2 ";
                    _pessoas.Add(p);
                    Thread.Sleep(300);
                }
            }            
        }

        private void PreencherListaPessoas2()
        {
            //O local correto para se tratar exceções em thread é o metodo usado pela thread, não onde ela fica
            try
            {
                //descomente a linha abaixo para ver como e possivel pegar exceção com thread
                //throw new Exception("Teste");
                IRepositorio<Pessoa> repositorioPessoas = new PessoaRepositorio();
                List<Pessoa> pessoas = repositorioPessoas.SelecionarTodos();
                //Use a instrução lock para sincronizar o acesso de thread com um recurso compartilhado
                lock (locker) //impede que ambas as thread manipulem o mesmo objeto (_pessoas) ao mesmo tempo
                {
                    foreach (Pessoa p in pessoas)
                    {
                        p.Nome += " Thread 3 ";
                        _pessoas.Add(p);
                        Thread.Sleep(100);
                    }
                }
            }
            catch(Exception ex)
            {
                ExibirErro(ex);
            }           
        }

        private void ExibirErro(Exception ex)
        {
            MessageBox.Show(ex.Message);
        }

        private void btnAdicionarPessoa_Click(object sender, EventArgs e)
        {
            FrmAdicionarPessoa frmAdicionarPessoa = new FrmAdicionarPessoa();
            frmAdicionarPessoa.ShowDialog();
            PreencherDataGridView();
        }
    }
}
