using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

namespace SHA256_encrypt_decrypt
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        byte[] bytesToBeEncrypted = null;
        byte[] bytesEncrypted = null;
        byte[] bytesToBeDecrypted = null;
        byte[] bytesDecrypted = null;
        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            /***********************Encryption**********************************************/
            // Get the bytes of the string
            if (textBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("text or private key is empty");
                return;
            }
            byte[] bytesToBeEncrypted = Encoding.UTF8.GetBytes(textBox1.Text);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(textBox2.Text);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string encryptedResult = Convert.ToBase64String(bytesEncrypted);
            richTextBox1.Text=encryptedResult;
            /***********************End*Encryption******************************************/
        }
        public static byte[] AES_Encrypt(byte[] bytesToBeEncrypted, byte[] passwordBytes)
        {
            byte[] encryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeEncrypted, 0, bytesToBeEncrypted.Length);
                        cs.Close();
                    }
                    encryptedBytes = ms.ToArray();
                }
            }

            return encryptedBytes;
        }
        public static byte[] AES_Decrypt(byte[] bytesToBeDecrypted, byte[] passwordBytes)
        {
            byte[] decryptedBytes = null;

            // Set your salt here, change it to meet your flavor:
            // The salt bytes must be at least 8 bytes.
            byte[] saltBytes = new byte[] { 2, 1, 7, 3, 6, 4, 8, 5 };

            using (MemoryStream ms = new MemoryStream())
            {
                using (RijndaelManaged AES = new RijndaelManaged())
                {
                    AES.KeySize = 256;
                    AES.BlockSize = 128;

                    var key = new Rfc2898DeriveBytes(passwordBytes, saltBytes, 1000);
                    AES.Key = key.GetBytes(AES.KeySize / 8);
                    AES.IV = key.GetBytes(AES.BlockSize / 8);

                    AES.Mode = CipherMode.CBC;

                    using (var cs = new CryptoStream(ms, AES.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(bytesToBeDecrypted, 0, bytesToBeDecrypted.Length);
                        cs.Close();
                    }
                    decryptedBytes = ms.ToArray();
                }
            }

            return decryptedBytes;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            /***********************Decryption**********************************************/
            // Get the bytes of the string
            if (richTextBox1.Text == "" || textBox2.Text == "")
            {
                MessageBox.Show("No encrypted text found or private key is empty");
                return;
            }
            byte[] bytesToBeDecrypted = Convert.FromBase64String(richTextBox1.Text);
            byte[] passwordBytes = Encoding.UTF8.GetBytes(textBox2.Text);
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            byte[] bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string decryptedResult = Encoding.UTF8.GetString(bytesDecrypted);
            richTextBox2.Text = decryptedResult;
            /***********************End*Decryption******************************************/
        }

        private void button3_Click(object sender, EventArgs e)
        {
            int size = -1;
            
            OpenFileDialog openFileDialog1 = new OpenFileDialog();
            DialogResult result = openFileDialog1.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog1.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                    bytesToBeEncrypted= File.ReadAllBytes(file);
                }
                catch (IOException)
                {
                }
            }
            textBox4.Text="size of file : "+ Convert.ToString(size); // <-- Shows file size 

            System.Threading.Thread.Sleep(300);

            /***********************Encryption**********************************************/
            // Get the bytes of the string
            if (textBox3.Text == "")
            {
                MessageBox.Show("private key is empty") ;
                return;
            }
            byte[] passwordBytes = Encoding.UTF8.GetBytes(textBox3.Text);

            // Hash the password with SHA256
            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            bytesEncrypted = AES_Encrypt(bytesToBeEncrypted, passwordBytes);

            string encryptedResult = Convert.ToBase64String(bytesEncrypted);
            richTextBox3.Text = encryptedResult;
            /***********************End*Encryption******************************************/

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (bytesEncrypted == null)
            {
                MessageBox.Show("No File Encrypted yet");
                return;
            }
            SaveFileDialog file = new SaveFileDialog();
            file.ShowDialog();
            if (file.FileName != "")
            {
                File.WriteAllBytes(file.FileName, bytesEncrypted);
                MessageBox.Show("Saved to " + file.FileName);
            }
        }

        private void button5_Click(object sender, EventArgs e)
        {
            int size = -1;

            OpenFileDialog openFileDialog3 = new OpenFileDialog();
            DialogResult result = openFileDialog3.ShowDialog(); // Show the dialog.
            if (result == DialogResult.OK) // Test result.
            {
                string file = openFileDialog3.FileName;
                try
                {
                    string text = File.ReadAllText(file);
                    size = text.Length;
                    bytesToBeDecrypted = File.ReadAllBytes(file);
                }
                catch (IOException)
                {
                }
            }
            textBox5.Text = "size of file : " + Convert.ToString(size); // <-- Shows file size 

            System.Threading.Thread.Sleep(300);

            /***********************Decryption**********************************************/
            // Get the bytes of the string
            if (textBox3.Text == "")
            {
                MessageBox.Show("private key is empty");
                return;
            }
            byte[] passwordBytes = Encoding.UTF8.GetBytes(textBox3.Text);

            passwordBytes = SHA256.Create().ComputeHash(passwordBytes);

            bytesDecrypted = AES_Decrypt(bytesToBeDecrypted, passwordBytes);

            string decryptedResult = Encoding.UTF8.GetString(bytesDecrypted);
            richTextBox4.Text = decryptedResult;
            /***********************End*Decryption******************************************/
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (bytesDecrypted == null)
            {
                MessageBox.Show("No File decrypted bytes found yet");
                return;
            }
            SaveFileDialog file = new SaveFileDialog();
            file.ShowDialog();
            if (file.FileName != "")
            {
                File.WriteAllBytes(file.FileName, bytesDecrypted);
                MessageBox.Show("Saved to " + file.FileName);
            }
        }
    }
}
