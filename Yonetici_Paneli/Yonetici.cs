﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;

namespace Yönetici_Paneli
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        static string BaglantiAdresi = "Server=**.**.*.**\\SQLEXPRESS;Initial Catalog=siramatik;MultipleActiveResultSets=true;User Id=****;Password=****;";
        SqlConnection baglanti = new SqlConnection(BaglantiAdresi);
        DataTable dt = new DataTable();
        DataTable dt_bolum = new DataTable();
        DataTable dt_check = new DataTable();
        DataTable dt_list = new DataTable();
        private void Btn_kaydet_Click(object sender, EventArgs e)
        {
            if (txt_tc.Text == "")
            {
                MessageBox.Show("Bilgiler boş bırakılmamalıdır!");
            }
            else if (txt_isim.Text == "")
            {
                MessageBox.Show("Bilgiler boş bırakılmamalıdır!");
            }
            else if (txt_soyisim.Text == "")
            {
                MessageBox.Show("Bilgiler boş bırakılmamalıdır!");
            }
            else if (txt_sifre.Text == "")
            {
                MessageBox.Show("Bilgiler boş bırakılmamalıdır!");
            }
            else
            {
                SqlCommand komut2 = new SqlCommand("Select * From Kullanicilar Where kimlik=@p1", baglanti);
                komut2.Parameters.AddWithValue("@p1",txt_tc.Text);
                baglanti.Open();
                SqlDataReader dr = komut2.ExecuteReader();
                if (dr.Read())
                {
                    MessageBox.Show("GİRDİĞİNİZ KİMLİK NUMARASI KULLANILMAKTADIR", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    dr.Close();
                    baglanti.Close();
                    Kaydet();
                }dr.Close();
                baglanti.Close();
                dt.Clear();
                dt_check.Clear();
                dt_list.Clear();
                dt_bolum.Clear();
                listBox1.Refresh();
                listBox2.Refresh();
                dataGridView2.Refresh();
                dataGridView1.Refresh();
                Form1_Load(null, null);
            }
        }

        private void Btn_temizle_Click(object sender, EventArgs e)
        {
            Temizle();
        }

        private void Btn_sil_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen Silmek İstediğiniz Kullanıcıyı Seçiniz.");
            }
            else
            {
                if (MessageBox.Show("Silmek İstediğinize Emin Misiniz?", "UYARI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow drow in dataGridView1.SelectedRows)  //Seçili Satırları Silme
                    {
                        int numara = Convert.ToInt32(drow.Cells[0].Value);
                        KayıtSil(numara);
                    }
                    dt.Clear();
                    dt_bolum.Clear();
                    dt_list.Clear();
                    dt_check.Clear();
                    listBox1.Refresh();
                    listBox2.Refresh();
                    dataGridView2.Refresh();
                    dataGridView1.Refresh();
                    Form1_Load(null, null);
                }
                else
                {
                    MessageBox.Show("İşlem tarafınızca iptal edilmiştir.", "İptal", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        public void Kaydet()
        {
            baglanti.ConnectionString = BaglantiAdresi;
            string kayit = "insert into Kullanicilar(isim,soyisim,kimlik,sifre) values (@isim,@soyisim,@kimlik,@sifre)";
            SqlCommand komut = new SqlCommand(kayit, baglanti);
            komut.Parameters.AddWithValue("@isim", txt_isim.Text);
            komut.Parameters.AddWithValue("@soyisim", txt_soyisim.Text);
            komut.Parameters.AddWithValue("@kimlik", txt_tc.Text);
            komut.Parameters.AddWithValue("@sifre", txt_sifre.Text);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        public void Temizle()
        {
            txt_isim.Clear();
            txt_soyisim.Clear();
            txt_sifre.Clear();
            txt_tc.Clear();
        }
        void KayıtSil(int numara)
        {
            string sql = "DELETE FROM Kullanicilar WHERE id=@id";
            string sql2 = "DELETE FROM Kullanici_Bolum WHERE kullanici_id=@id2";
            SqlCommand komut2 = new SqlCommand(sql2, baglanti);
            SqlCommand komut = new SqlCommand(sql, baglanti);
            komut2.Parameters.AddWithValue("@id2", numara);
            komut.Parameters.AddWithValue("@id", numara);
            baglanti.Open();
            komut2.ExecuteNonQuery();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            baglanti.ConnectionString = BaglantiAdresi;
            string kayit = "SELECT id,isim,soyisim,kimlik From Kullanicilar ";
            string bolum_tablo = "SELECT bolum_id,bolum_adi From Bolumler";
            baglanti.Open();
            SqlDataAdapter adapter = new SqlDataAdapter(kayit, baglanti);
            SqlDataAdapter adp_bolum = new SqlDataAdapter(bolum_tablo, baglanti);
            SqlDataAdapter adp_check = new SqlDataAdapter(bolum_tablo, baglanti);
            SqlDataAdapter adp_list = new SqlDataAdapter(kayit, baglanti);
            adp_bolum.Fill(dt_bolum);
            adp_check.Fill(dt_check);
            adapter.Fill(dt);
            adp_list.Fill(dt_list);

            listBox1.DataSource = dt_list;
            listBox1.DisplayMember = "kimlik";
            listBox1.ValueMember = "id";

            listBox2.DataSource = dt_list;
            listBox2.DisplayMember = "isim";
            listBox2.ValueMember = "id";

            checkedListBox1.DataSource = dt_check;
            checkedListBox1.DisplayMember = "bolum_adi";
            checkedListBox1.ValueMember = "bolum_id";

            dataGridView1.DataSource = dt;
            dataGridView2.DataSource = dt_bolum;
            baglanti.Close();

            listBox2.SelectedIndex = -1;
            listBox1.SelectedIndex = -1;
        }

        private void Txt_tc_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "Convert([kimlik], System.String) Like '%" + txt_tc.Text.ToString()+ "%'";
            dataGridView1.DataSource = dt;
        }

        private void Txt_isim_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "isim Like '%" + txt_isim.Text + "%'";
            dataGridView1.DataSource = dt;
        }

        private void Txt_soyisim_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt.DefaultView;
            dv.RowFilter = "soyisim Like '%" + txt_soyisim.Text.ToString() + "%'";
            dataGridView1.DataSource = dt;
        }
        public void Bolum_Kaydet()
        {
            baglanti.ConnectionString = BaglantiAdresi;
            string kayit = "insert into Bolumler(bolum_adi) values (@bolum_adi)";
            SqlCommand komut = new SqlCommand(kayit, baglanti);
            komut.Parameters.AddWithValue("@bolum_adi", txt_bolum.Text);
            baglanti.Open();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }
        public void Bolum_Sil(int numara)
        {
            string sql = "DELETE FROM Bolumler WHERE bolum_id=@bolum_id";
            string sql2 = "DELETE FROM Kullanici_Bolum WHERE bolum_id=@bolum_id2";
            SqlCommand komut2= new SqlCommand(sql2, baglanti);
            SqlCommand komut = new SqlCommand(sql, baglanti);
            komut2.Parameters.AddWithValue("@bolum_id2", numara);
            komut.Parameters.AddWithValue("@bolum_id", numara);
            baglanti.Open();
            komut2.ExecuteNonQuery();
            komut.ExecuteNonQuery();
            baglanti.Close();
        }

        private void Btn_bolumkaydet_Click(object sender, EventArgs e)
        {
            if (txt_bolum.Text == "")
            {
                MessageBox.Show("Boş bırakma");
            }
            else
            {
                SqlCommand komut2 = new SqlCommand("Select * From Bolumler Where bolum_adi=@p1", baglanti);
                komut2.Parameters.AddWithValue("@p1", txt_bolum.Text);
                baglanti.Open();
                SqlDataReader dr = komut2.ExecuteReader();
                if (dr.Read())
                {
                    MessageBox.Show("GİRDİĞİNİZ BÖLÜM KULLANILMAKTADIR", "UYARI", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    dr.Close();
                    baglanti.Close();
                    Bolum_Kaydet();
                }
                dr.Close();
                baglanti.Close();
                dt.Clear();
                dt_bolum.Clear();
                dt_list.Clear();
                dt_check.Clear();
                listBox1.Refresh();
                listBox2.Refresh();
                dataGridView2.Refresh();
                dataGridView1.Refresh();
                Form1_Load(null,null);
                txt_bolum.Clear();
            }
        }

        private void Txt_bolum_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt_bolum.DefaultView;
            dv.RowFilter = "bolum_adi Like '%" + txt_bolum.Text.ToString() + "%'";
            dataGridView1.DataSource = dt_bolum;
        }

        private void Btn_bolumsil_Click(object sender, EventArgs e)
        {
            if (dataGridView2.SelectedRows.Count == 0)
            {
                MessageBox.Show("Lütfen Silmek İstediğiniz Bölümü Seçiniz.");
            }
            else
            {
                if (MessageBox.Show("Silmek İstediğinize Emin Misiniz?", "UYARI", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                {
                    foreach (DataGridViewRow drow in dataGridView2.SelectedRows)  //Seçili Satırları Silme
                    {
                        int numara = Convert.ToInt32(drow.Cells[0].Value);
                        Bolum_Sil(numara);
                    }
                    dt.Clear();
                    dt_bolum.Clear();
                    dt_list.Clear();
                    dt_check.Clear();
                    listBox1.Refresh();
                    listBox2.Refresh();
                    dataGridView2.Refresh();
                    dataGridView1.Refresh();
                    Form1_Load(null, null);
                }
                else
                {
                    MessageBox.Show("İşlem tarafınızca iptal edilmiştir.", "İptal", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void Txt_bul_TextChanged(object sender, EventArgs e)
        {
            DataView dv = dt_list.DefaultView;
            dv.RowFilter = "Convert([kimlik], System.String)Like '%" + txt_bul.Text.ToString() + "%'" +
                "OR isim Like '%" + txt_bul.Text.ToString() + "%'" +
                "OR soyisim Like '%" + txt_bul.Text.ToString() + "%'";
            listBox1.DataSource = dt_list;
            listBox2.DataSource = dt_list;
        }

        private void Btn_ata_Click(object sender, EventArgs e)
        {
            Bolum_Degis();
            MessageBox.Show("Kayıt İşlemi Başarılı");
        }
        public void Bolum_Degis()
        {
            //KAYITLI BÖLÜMLERİ SİL
            baglanti.ConnectionString = BaglantiAdresi;
            SqlCommand silen = new SqlCommand("DELETE FROM Kullanici_Bolum Where kullanici_id=@p13", baglanti);
            silen.Parameters.AddWithValue("@p13", Convert.ToInt32(listBox1.SelectedValue));
            baglanti.Open();
            silen.ExecuteNonQuery();
            baglanti.Close();

            //CHECKEDLİSTBOXTA İŞARETLENEN VERİLERİ KAYDET
            List<string> checkedItems = new List<string>();
            foreach (var item in checkedListBox1.CheckedItems)
                checkedItems.Add(((System.Data.DataRowView)(item)).Row.ItemArray[0].ToString());

            baglanti.Open();
            foreach (var item in checkedItems)
            {
                string kayit = "insert into Kullanici_Bolum(bolum_id,kullanici_id) values (@bolum_id,@kullanici_id)";
                SqlCommand komut = new SqlCommand(kayit, baglanti);
                komut.Parameters.AddWithValue("@kullanici_id", Convert.ToInt32(listBox1.SelectedValue));
                komut.Parameters.AddWithValue("@bolum_id", item);
                komut.ExecuteNonQuery();
            }
            baglanti.Close();
        }
        
        private void TabPage3_Leave(object sender, EventArgs e)
        {
            //listBox2.SelectedIndex = -1;
            //listBox1.SelectedIndex = -1;
        }

        private void ListBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                for (int i = 0; i < checkedListBox1.Items.Count; i++)
                {
                    checkedListBox1.SetItemCheckState(i, CheckState.Unchecked);
                }
                if (baglanti.State == ConnectionState.Closed)
                    baglanti.Open();

                SqlCommand komutdr = new SqlCommand("Select bolum_id from Kullanici_Bolum Where kullanici_id=@p1", baglanti);
                komutdr.Parameters.AddWithValue("@p1", listBox1.SelectedValue);
                SqlDataReader dr = komutdr.ExecuteReader();
                while (dr.Read())
                {
                    for (int i = 0; i < checkedListBox1.Items.Count; i++)
                    {
                        if (((System.Data.DataRowView)(checkedListBox1.Items[i])).Row.ItemArray[0].ToString() == dr[0].ToString())

                            checkedListBox1.SetItemCheckState(i, CheckState.Checked);
                    }
                }
                dr.Close();
                baglanti.Close();
            }
        }
    }
}
