using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.OleDb;

namespace UdovicicOP_LV4
{
    public partial class Form1 : Form
    {             

        DataTable dt = new DataTable();
        DataTable dtem = new DataTable();
        DataTable dtsh = new DataTable();

        OleDbConnection myConnection;
        OleDbDataAdapter myOleDbAdapter;
  
        DataSet dsOrders;
       
        private const string myConnectionString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=C:\Users\ACER7\Desktop\UdovicicOP_LV4\UdovicicOP_LV4\Northwind.MDB";
        public Form1()
        {
            InitializeComponent();
            
            BindCustomer();
            BindShipper();
            BindEmployer();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.employeesTableAdapter.Fill(this.northwindDataSet.Employees);
            RefreshData();
        }

        private void RefreshData()   //ISPUNJAVA TABLICU ORDERS PODATCIMA(DATASET)
        {
            using (myConnection = new OleDbConnection(myConnectionString))
            {
                myConnection.Open();
                using (myOleDbAdapter = new OleDbDataAdapter())
                {

                    dsOrders = new DataSet(); 
                    myOleDbAdapter.SelectCommand = new OleDbCommand("Select * FROM Orders", myConnection);
                    myOleDbAdapter.Fill(dsOrders);
                    dataGridView1.DataSource = dsOrders.Tables[0];

                }

                myConnection.Close();

            }

        }

        private void BindCustomer()  //POPUNJAVA COMBOBOX CUSTOMER(DATATABLE) 
        {
            string s = "SELECT * FROM Customers";

            using (OleDbConnection dbcon = new OleDbConnection(myConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(s, dbcon))
            {
             
                dbcon.Open();
                dt.Load(cmd.ExecuteReader());

                comboBoxCustomer.DisplayMember = "CompanyName";
                comboBoxCustomer.ValueMember = "CustomerID";
                comboBoxCustomer.DataSource = dt;

            }

        }
        private void BindShipper()   //POPUNJAVA COMBOBOX SHIPPER(DATATABLE) 
        {
          string s = "SELECT * FROM Shippers";

            using (OleDbConnection dbcon = new OleDbConnection(myConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(s, dbcon))
            {

                dbcon.Open();

                dtsh.Load(cmd.ExecuteReader());

                comboBoxShipper.DisplayMember = "CompanyName";
                comboBoxShipper.ValueMember = "ShipperID";  
                comboBoxShipper.DataSource = dtsh;

            }
        }
        private void BindEmployer()   //POPUNJAVA COMBOBOX EMPLOYER(DATATABLE) 
        {
            string s = "SELECT * FROM Employees";   

            using (OleDbConnection dbcon = new OleDbConnection(myConnectionString))
            using (OleDbCommand cmd = new OleDbCommand(s, dbcon))
            {
                dbcon.Open();

                dtem.Load(cmd.ExecuteReader());

                comboBoxEmployer.DisplayMember = "LastName";
                comboBoxEmployer.ValueMember = "EmployeeID";
                comboBoxEmployer.DataSource = dtem;

            }

        }

        private void comboBoxCustomer_SelectedIndexChanged(object sender, EventArgs e)  //FILTRIRA DATAVIEWGRID PREMA IMENU CUSTOMERA, A U VIEW PRIKAZUJE NJEGOV ID
        {   
  
                Console.WriteLine("The value of {0} is {1}", comboBoxCustomer.Text, comboBoxCustomer.SelectedValue); 

                OleDbDataAdapter olead = new OleDbDataAdapter("Select * From Orders Where CustomerID Like '" + comboBoxCustomer.SelectedValue + "%' ", myConnectionString);
                DataTable dt = new DataTable();
                olead.Fill(dt);
                dataGridView1.DataSource = dt;

        }
    

        private void comboBoxShipper_SelectedIndexChanged(object sender, EventArgs e)  //FILTRIRANJE SHIPPERA
        {
             
                Console.WriteLine("The value of {0} is {1}", comboBoxShipper.Text, comboBoxShipper.SelectedValue);

                OleDbDataAdapter olead = new OleDbDataAdapter("Select * From Orders Where ShipVia Like '" + comboBoxShipper.SelectedValue + "%' ", myConnectionString);
                DataTable dt = new DataTable();
                olead.Fill(dt);
                dataGridView1.DataSource = dt;
        }

        private void comboBoxEmployer_SelectedIndexChanged(object sender, EventArgs e)  //FILTRIRANJE EMPLOYEE
        {

           Console.WriteLine("The value of {0} is {1}", comboBoxEmployer.Text, comboBoxEmployer.SelectedValue); 

           OleDbDataAdapter olead = new OleDbDataAdapter("Select * From Orders Where EmployeeID Like '" + comboBoxEmployer.SelectedValue + "%' ", myConnectionString);
           DataTable dt = new DataTable();
           olead.Fill(dt);
           dataGridView1.DataSource = dt;
        }

      
        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e){}

        private void comboBoxEmployer_ValueMemberChanged(object sender, EventArgs e){}

        private void comboBoxEmployer_SelectedValueChanged(object sender, EventArgs e){}

     
        private void deleteButton_Click(object sender, EventArgs e)  //BUTTON ZA BRISANJE
        {
            if (MessageBox.Show("Are you sure want to delete this record?", "Message", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {  
            using (myConnection = new OleDbConnection(myConnectionString))
            {
                myConnection.Open();
                try
                {
                    string q = " DELETE FROM Orders WHERE [OrderID] =" + textBox1.Text;
                    OleDbCommand My_Command = new OleDbCommand(q, myConnection);
                    My_Command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                }
                myConnection.Close();
            }

            RefreshData();
            MessageBox.Show("DELETED SUCCESSFULLY!");   
           }

        }

        private void updateButton_Click(object sender, EventArgs e)  //OMOGUĆAVA IZMJENU PODATAKA, NE RADI PRI PROMJENI TEZINE JER AUTOMATSKI ISPUNJAVA DA JE VRIJEDNOST 0
        {
           using (myConnection = new OleDbConnection(myConnectionString))
            {
                myConnection.Open();
                try
                {
                    string myString = "UPDATE Orders SET EmployeeID=@EmployeeID, CustomerID=@CustomerID, OrderDate=@OrderDate, RequiredDate=@RequiredDate, ShippedDate=@ShippedDate, ShipVia=@ShipVia, ShipName=@ShipName, ShipAddress=@ShipAddress, ShipCity=@ShipCity, ShipRegion=@ShipRegion, ShipPostalCode=@ShipPostalCode, ShipCountry=@ShipCountry WHERE OrderID=@ID";
                                
                    OleDbCommand myCom = new OleDbCommand(myString, myConnection);
                    myCom.Parameters.AddWithValue("@EmployeeID", textBox3.Text);
                    myCom.Parameters.AddWithValue("@CustomerID", textBox2.Text);      
                    myCom.Parameters.AddWithValue("@OrderDate", textBox4.Text);
                    myCom.Parameters.AddWithValue("@RequiredDate", textBox5.Text);
                    myCom.Parameters.AddWithValue("@ShippedDate", textBox6.Text);
                    myCom.Parameters.AddWithValue("@ShipVia", textBox7.Text);
                    myCom.Parameters.AddWithValue("@ShipName", textBox9.Text);
                    myCom.Parameters.AddWithValue("@ShipAddress", textBox10.Text);
                    myCom.Parameters.AddWithValue("@ShipCity", textBox11.Text);
                    myCom.Parameters.AddWithValue("@ShipRegion", textBox12.Text);
                    myCom.Parameters.AddWithValue("@ShipPostalCode", textBox13.Text);
                    myCom.Parameters.AddWithValue("@ShipCountry", textBox14.Text);
                    myCom.Parameters.AddWithValue("@ID", textBox1.Text);
                    myCom.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                }
                myConnection.Close();
            }

           RefreshData();
           MessageBox.Show("UPDATED SUCCESSFULLY!");

        }
        
        private void insertButton_Click(object sender, EventArgs e)  //DODAJE NARUDZBU (SVI TRAZENI PODATCI MORAJU BITI ISPUNJENI), NE RADI PRI PROMJENI TEZINE
        {
             using (myConnection = new OleDbConnection(myConnectionString))
            {
                myConnection.Open();
                try
                {
                    string query = "INSERT INTO Orders (OrderID, CustomerID, EmployeeID, OrderDate, RequiredDate, ShippedDate, ShipVia, ShipName, ShipAddress,ShipCity,ShipRegion, ShipPostalCode, ShipCountry) VALUES ('" + textBox1.Text + "','" + textBox2.Text + "','" + textBox3.Text + "','" + textBox4.Text + "','" + textBox5.Text + "','" + textBox6.Text + "','" + textBox7.Text + "', '" + textBox9.Text + "','" + textBox10.Text + "','" + textBox11.Text + "','" + textBox12.Text + "', '" + textBox13.Text + "', '" + textBox14.Text + "')";
                    OleDbDataAdapter oledb = new OleDbDataAdapter(query, myConnection);
                    oledb.SelectCommand.ExecuteNonQuery();
                    myConnection.Close();
                    MessageBox.Show("INSERTED SUCCESSFULLY!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK);
                }
                myConnection.Close();
            }

             RefreshData();
             MessageBox.Show("INSERTED SUCCESSFULLY!");
        }
      
      

        private String Query(String CustomerID, String ShipVia, String EmployeeID) //ODABIR NARUDZBE PREMA ODABIRU VRIJEDNOSTI IZ TRI COMBOBOXA
        {
            StringBuilder query = new StringBuilder();
            query.Append("SELECT * FROM Orders");
            if (CustomerID != "0")
            {
                query.Append(" WHERE CustomerID='").Append(CustomerID).Append("'");
                if (ShipVia != "0")
                {
                    query.Append(" AND ShipVia=").Append(ShipVia);
                }
                if (EmployeeID != "0")
                {
                    query.Append(" AND EmployeeID=").Append(EmployeeID);
                }
            }
            else if (ShipVia != "0")
            {
                query.Append(" WHERE ShipVia=").Append(ShipVia);
                if (EmployeeID != "0")
                {
                    query.Append(" AND EmployeeID=").Append(EmployeeID);
                }
            }
            else if (EmployeeID != "0")
            {
                query.Append(" WHERE EmployeeID=").Append(EmployeeID);
            }

            return query.ToString();
        }

        private void btnFilterAll_Click(object sender, EventArgs e)    //BUTTON KOJI FILTRIRA DATAGRIDVIEW S 3 COMBOBOXA
        {

            String CustomerID = comboBoxCustomer.SelectedValue.ToString();
            String ShipVia = comboBoxShipper.SelectedValue.ToString();
            String EmployeeID = comboBoxEmployer.SelectedValue.ToString();
            using (myConnection = new OleDbConnection(myConnectionString))
            {
                DataSet newdsOrders = new DataSet();
                using (myOleDbAdapter = new OleDbDataAdapter())
                {
                    myOleDbAdapter.SelectCommand = new OleDbCommand(Query(CustomerID, ShipVia, EmployeeID), myConnection);
                    myOleDbAdapter.Fill(newdsOrders);
                    dataGridView1.DataSource = null;
                    dataGridView1.DataSource =newdsOrders.Tables[0];
                }
  
            }

        }

        private void dataGridView1_MouseClick(object sender, MouseEventArgs e)  //KLIKOM NA DATAGRIDVIEW PRIKAZUJU SE SVI PODATCI U TEXTBOXU
        {
            textBox1.Text = dataGridView1.CurrentRow.Cells[0].Value.ToString();
            textBox2.Text = dataGridView1.CurrentRow.Cells[1].Value.ToString();
            textBox3.Text = dataGridView1.CurrentRow.Cells[2].Value.ToString();
            textBox4.Text = dataGridView1.CurrentRow.Cells[3].Value.ToString();
            textBox5.Text = dataGridView1.CurrentRow.Cells[4].Value.ToString();
            textBox6.Text = dataGridView1.CurrentRow.Cells[5].Value.ToString();
            textBox7.Text = dataGridView1.CurrentRow.Cells[6].Value.ToString();
            textBox8.Text = dataGridView1.CurrentRow.Cells[7].Value.ToString();
            textBox9.Text = dataGridView1.CurrentRow.Cells[8].Value.ToString();
            textBox10.Text = dataGridView1.CurrentRow.Cells[9].Value.ToString();
            textBox11.Text = dataGridView1.CurrentRow.Cells[10].Value.ToString();
            textBox12.Text = dataGridView1.CurrentRow.Cells[11].Value.ToString();
            textBox13.Text = dataGridView1.CurrentRow.Cells[12].Value.ToString();
            textBox14.Text = dataGridView1.CurrentRow.Cells[13].Value.ToString();
                
        }


        private void viewButton_Click(object sender, EventArgs e)  //BUTTON KOJI OSVJEZAVA TABLICU
        {
            RefreshData();
        }

 
    }
}