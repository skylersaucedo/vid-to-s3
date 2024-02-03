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
    public partial class frmMain : Form
    {
        string selectedFileName { get; set; }

        AWSCredentials awsCredentials;
        public RegionEndpoint region = RegionEndpoint.USEast1;
        public string bucket = "tsi-raw-videos";
        public string bucketSubDir = "test-subdir"; // Optional subdirectory within the bucket
        public string fileNameInS3 = "testvideo-feb3.avi"; // Desired file name in S3

        public static async Task sendVidFile(string fname, string bucketName, string bucketSubDirName, string S3fname, RegionEndpoint r)
        {

            var client = new AmazonS3Client(r); 
            var transferUtility = new TransferUtility(client);
            await transferUtility.UploadAsync(fname, bucketName, $"{bucketSubDirName}/{S3fname}");
            Console.WriteLine("File uploaded successfully!");
        }

        private void btnSendVideo_Click(object sender, EventArgs e)
        {
            //send video to S3

            if (selectedFileName != "")
            {
                Console.WriteLine("you selected: " + selectedFileName);
                _ = sendVidFile(selectedFileName, bucket, bucketSubDir, fileNameInS3, region);
            }

            else
            {
                MessageBox.Show("No file selected...");
            }
        }

        public frmMain()
        {
            InitializeComponent();

            var chain = new CredentialProfileStoreChain();

            if (chain.TryGetAWSCredentials("videouploader", out awsCredentials))
            {
                Console.WriteLine("Credentials profile found...");
                Console.WriteLine(awsCredentials.ToString());
            }
        }

        private void btnSelectVideo_Click(object sender, EventArgs e)
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
    }
}
