using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.Runtime.CredentialManagement;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Transfer;
using System.Xml.Linq;

//
// AWS credentials done in AWS CLI. Use CMD and run 'aws configure' to set. 
//
namespace vid_to_s3
{
    public partial class Form1 : Form
    {
        AWSCredentials awsCredentials;
        public string bucketName = "tsi-raw-videos";
        public string subDirectoryInBucket = "test-subdir"; // Optional subdirectory within the bucket
        public string fileNameInS3 = "testvideo-feb2-2.avi"; // Desired file name in S3

        public Form1()
        {
            InitializeComponent();

            var chain = new CredentialProfileStoreChain();

            if (chain.TryGetAWSCredentials("videouploader", out awsCredentials))
            {
                MessageBox.Show("Credentials profile found...");
                MessageBox.Show(awsCredentials.ToString());
            }
        }

        string selectedFileName { get; set; }

        private void btnSendVideo_Click(object sender, EventArgs e)
        {

            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.InitialDirectory = "C:\\";
                ofd.Filter = "Movie files (*.avi) | *.avi";
                ofd.FilterIndex = 0;
                ofd.RestoreDirectory = true;

                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    selectedFileName = ofd.FileName;
                    textBox1.Text = selectedFileName;
                }

                MessageBox.Show("you selected: " + selectedFileName);
            }
           
        }

        private void btnSendVideo_Click_1(object sender, EventArgs e)
        {

            //send video to S3

            if (selectedFileName != "")
            {
                MessageBox.Show("you selected: " + selectedFileName);
                MessageBox.Show("sending it!");
                _ = sendVidFile(selectedFileName, bucketName, subDirectoryInBucket, fileNameInS3);
                
            }
        }

        public static async Task sendVidFile(string fname, string bucketName, string subDirectoryInBucket, string fileNameInS3)
        {
            
            var client = new AmazonS3Client(RegionEndpoint.USEast1); // Choose your desired region

            var transferUtility = new TransferUtility(client);

            await transferUtility.UploadAsync(fname, bucketName, $"{subDirectoryInBucket}/{fileNameInS3}");

            Console.WriteLine("File uploaded successfully!");
        }
    }
}
