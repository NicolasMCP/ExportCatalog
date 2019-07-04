using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

/*
 * Autor: Herley Nicolas Ramos Sanchez
 * email: nicolas.mcp@gmail.com
 * 
 * licença:  GNU GPL 3
 * Objetivo: facilitar a importação de arquivos "copiados do cabeçalho de um web-mail" para
 *           criar um catálogo de endereços no Thunderbird
 */

namespace WindowsFormsApplication2 {

    public partial class Form1: Form {

        public Form1() {

            InitializeComponent();

        }

        private void BtnExport_Click(object sender, EventArgs e) {

            string s = txtDat.Text.Trim();
            if(s.Length > 0) {
                saveFileDialog1.Filter = "Valores Separados por Vírgula (*.csv)|*.csv";
                saveFileDialog1.DefaultExt = "csv";
                saveFileDialog1.AddExtension = true;
                saveFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                saveFileDialog1.ShowDialog();
            } else {
                MessageBox.Show("Para exportar você deve ter conteúdo relevante!", "Aviso!",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

        }

        private void SaveFileDialog1_FileOk(object sender, CancelEventArgs e) {

            string caminho = saveFileDialog1.FileName;
            StringBuilder sb = new StringBuilder();
            sb.Append(txtDat.Text);
            sb.Replace("\r","");
            
            File.WriteAllText(caminho, sb.ToString());
        }

        private void BtnFim_Click(object sender, EventArgs e) {

            this.Close();

        }

        private void BtnImport_Click(object sender, EventArgs e) {

            openFileDialog1.Filter = "Arquivos de Texto (*.txt)|*.txt";
            openFileDialog1.DefaultExt = "txt";
            openFileDialog1.AddExtension = true;
            openFileDialog1.FileName = "";
            openFileDialog1.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            openFileDialog1.ShowDialog();

        }

        private void OpenFileDialog1_FileOk(object sender, CancelEventArgs e) {

            btnImport.Enabled = false;
            string caminho = openFileDialog1.FileName;
            txtDat.Text = File.ReadAllText(caminho);

        }

        private void BtnProcessar_Click(object sender, EventArgs e) {

            string s;
            int menorQue;
            int arroba;
            int espaco;
            string umaLinha;
            string nome;

            btnProcessar.Enabled = false;
            btnImport.Enabled = false;

            StringBuilder sb = new StringBuilder();
            StringBuilder sbFinal = new StringBuilder();

            sb.Append(txtDat.Text);
            s = CortesLinha(sb);

            if(s.Substring(0, 13) != "Primeiro nome,") {

                sbFinal.Append("Primeiro nome,Sobrenome,Apresentar como,Apelido,E-mail primário,E-mail secundário,Nome de tela,Telefone comercial,Telefone residencial,Número do fax,Número do pager,Número celular,Endereço,Endereço 2,Cidade,Estado,CEP,País,Endereço de trabalho,Endereço de trabalho 2,Cidade de trabalho,Estado de trabalho,CEP de trabalho,País de trabalho,Cargo,Setor,Empresa,Página da web 1,Página da web 2,Ano de nascimento,Mês de nascimento,Nascimento,Personalizado 1,Personalizado 2,Personalizado 3,Personalizado 4,Observações,\n");
            }

            do {
                umaLinha = ExtraiUmaLinha(s);

                if((umaLinha.Length) < s.Length) {
                    s = s.Remove(0, umaLinha.Length);
                } else {
                    s = "";
                }
                
                menorQue = umaLinha.IndexOf("<");
                arroba = umaLinha.IndexOf("@");
                espaco = umaLinha.IndexOf(" ");

                if(menorQue == -1) {

                    menorQue = int.MaxValue;
                }

                if(espaco == -1) {

                    espaco = int.MaxValue;
                }

                if(arroba == -1) {

                    MessageBox.Show("Há um erro no formato dos dados", "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if(arroba < menorQue) {

                    if(espaco < arroba) {

                        MessageBox.Show("Há um erro no formato dos dados", "Aviso!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;

                    } else {

                        nome = umaLinha.Substring(0, arroba);
                        sb.Append(nome + ",," + nome + ",," + umaLinha);
                        sb.Replace("|", ",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,");
                    }

                } else {

                    if(espaco == (menorQue - 1)){

                        nome = umaLinha.Substring(0, espaco);
                        sb.Append(nome + ",," + umaLinha);
                        sb.Replace(" <", ",,");
                        sb.Replace("|", ",,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,");
                    }

                }
                sbFinal.Append(sb.ToString());
                sb.Clear();
                sbFinal.Replace("\n", "\r\n");
                txtDat.Text = sbFinal.ToString();
                label1.Text = "Dados Processados Corretamente!";

            } while (s.Length > 3);

        }

        private string ExtraiUmaLinha(string s) {

            s = s.Substring(0, s.IndexOf("|\n") + 2);
            return s;
        }

        private string CortesLinha(StringBuilder sb) {

            int final;
            string s;
            sb.Replace(">, ", "|\n");
            sb.Replace(", ", "|\n");
            s = sb.ToString();
            sb.Clear();

            final = s.Length - 2;
            if(!s.Substring(final, 2).Equals("|\n")) {
                if(s.Substring(final + 1, 1).Equals(">")|| s.Substring(final + 1, 1).Equals(",")) {
                    s = s.Remove(final + 1, 1);
                }
                s += "|\n";
            }
            return s;
        }
    }
}
