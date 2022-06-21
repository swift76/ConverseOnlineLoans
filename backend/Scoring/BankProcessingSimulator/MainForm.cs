using System;
using System.Configuration;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;
using Dapper;

namespace BankProcessingSimulator
{
    public partial class MainForm : Form
    {
        string ConnectionString;

        public MainForm()
        {
            InitializeComponent();
            ConnectionString = ConfigurationManager.ConnectionStrings["ScoringDB"].ConnectionString;
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void buttonApprove_Click(object sender, EventArgs e)
        {
            SetStatus(5);
        }

        private void buttonRefuse_Click(object sender, EventArgs e)
        {
            SetStatus(6);
        }

        private void SetStatus(int status)
        {
            using (IDbConnection connection = new SqlConnection(ConnectionString))
            {
                if (connection.State != ConnectionState.Open)
                    connection.Open();

                IEnumerable<Guid> ids = connection.Query<Guid>("select ID from dbo.APPLICATION where STATUS_ID=" + (status == 21 ? 15 : 1), commandType: CommandType.Text);
                foreach (Guid id in ids)
                {
                    switch (status)
                    {
                        case 5:
                            connection.Execute("insert into dbo.APPLICATION_SCORING_RESULT (APPLICATION_ID,SCORING_NUMBER,AMOUNT,INTEREST) values ('" + id.ToString() + "', 0, 300000, 15.123)", commandType: CommandType.Text);
                            if (checkHasMultipleOptions.Checked)
                                connection.Execute("insert into dbo.APPLICATION_SCORING_RESULT (APPLICATION_ID,SCORING_NUMBER,AMOUNT,INTEREST) values ('" + id.ToString() + "', 1, 200000, 18)", commandType: CommandType.Text);
                            connection.Execute("update dbo.APPLICATION set STATUS_ID=5,CLIENT_CODE='" + textClientCode.Text + "',HAS_BANK_CARD=" + (checkHasCards.Checked ? "1" : "0") + ",IS_DATA_COMPLETE=" + (checkIsDataComplete.Checked ? "1" : "0") + " where ID='" + id.ToString() + "'", commandType: CommandType.Text);
                            break;
                        case 6:
                            connection.Execute("update dbo.APPLICATION set REFUSAL_REASON=N'Սխալ փաստաթղթի տվյալներ',STATUS_ID=0 where ID='" + id.ToString() + "'", commandType: CommandType.Text);
                            break;
                        case 7:
                            connection.Execute("update dbo.APPLICATION set MANUAL_REASON=N'Վերստուգման կարիք կա',STATUS_ID=7 where ID='" + id.ToString() + "'", commandType: CommandType.Text);
                            break;
                        case 21:
                            connection.Execute("update dbo.APPLICATION set STATUS_ID=21 where ID='" + id.ToString() + "'", commandType: CommandType.Text);
                            break;
                    }
                }
            }
        }

        private void buttonManual_Click(object sender, EventArgs e)
        {
            SetStatus(7);
        }

        private void buttonGive_Click(object sender, EventArgs e)
        {
            SetStatus(21);
        }
    }
}
