namespace WinFormsAppCore
{
    partial class Main
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            txtAddress = new TextBox();
            txtUser = new TextBox();
            txtPwd = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            btn_Connect = new Button();
            btn_GetNodes = new Button();
            txtDataInfo = new RichTextBox();
            txtTargetNode = new TextBox();
            textBox3 = new TextBox();
            btnSyncRead = new Button();
            btnAsyncRead = new Button();
            label4 = new Label();
            btnSubscription = new Button();
            btnCloseMo = new Button();
            btnWrite = new Button();
            btnSlmWriter = new Button();
            btnNodeAtrr = new Button();
            txtLog = new RichTextBox();
            butRead = new Button();
            button1 = new Button();
            label6 = new Label();
            NodeTree = new CheckedListBox();
            label5 = new Label();
            label7 = new Label();
            label8 = new Label();
            SuspendLayout();
            // 
            // txtAddress
            // 
            txtAddress.Location = new Point(79, 18);
            txtAddress.Margin = new Padding(5, 4, 5, 4);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new Size(441, 30);
            txtAddress.TabIndex = 0;
            // 
            // txtUser
            // 
            txtUser.Location = new Point(591, 18);
            txtUser.Margin = new Padding(5, 4, 5, 4);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(155, 30);
            txtUser.TabIndex = 1;
            // 
            // txtPwd
            // 
            txtPwd.Location = new Point(817, 18);
            txtPwd.Margin = new Padding(5, 4, 5, 4);
            txtPwd.Name = "txtPwd";
            txtPwd.Size = new Size(155, 30);
            txtPwd.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(16, 21);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(46, 24);
            label1.TabIndex = 3;
            label1.Text = "地址";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(531, 21);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(46, 24);
            label2.TabIndex = 4;
            label2.Text = "账号";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(757, 21);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(46, 24);
            label3.TabIndex = 5;
            label3.Text = "密码";
            // 
            // btn_Connect
            // 
            btn_Connect.Location = new Point(985, 17);
            btn_Connect.Margin = new Padding(5, 4, 5, 4);
            btn_Connect.Name = "btn_Connect";
            btn_Connect.Size = new Size(118, 32);
            btn_Connect.TabIndex = 6;
            btn_Connect.Text = "连接";
            btn_Connect.UseVisualStyleBackColor = true;
            btn_Connect.Click += btnConnect_Click;
            // 
            // btn_GetNodes
            // 
            btn_GetNodes.Location = new Point(830, 69);
            btn_GetNodes.Margin = new Padding(5, 4, 5, 4);
            btn_GetNodes.Name = "btn_GetNodes";
            btn_GetNodes.Size = new Size(142, 32);
            btn_GetNodes.TabIndex = 7;
            btn_GetNodes.Text = "获取节点信息";
            btn_GetNodes.UseVisualStyleBackColor = true;
            btn_GetNodes.Click += btn_GetNode_Click;
            // 
            // txtDataInfo
            // 
            txtDataInfo.Location = new Point(591, 214);
            txtDataInfo.Margin = new Padding(5, 4, 5, 4);
            txtDataInfo.MinimumSize = new Size(586, 337);
            txtDataInfo.Name = "txtDataInfo";
            txtDataInfo.Size = new Size(586, 337);
            txtDataInfo.TabIndex = 9;
            txtDataInfo.Text = "";
            // 
            // txtTargetNode
            // 
            txtTargetNode.Location = new Point(145, 69);
            txtTargetNode.Margin = new Padding(5, 4, 5, 4);
            txtTargetNode.Name = "txtTargetNode";
            txtTargetNode.Size = new Size(661, 30);
            txtTargetNode.TabIndex = 12;
            // 
            // textBox3
            // 
            textBox3.Location = new Point(145, 116);
            textBox3.Margin = new Padding(5, 4, 5, 4);
            textBox3.Name = "textBox3";
            textBox3.Size = new Size(658, 30);
            textBox3.TabIndex = 14;
            // 
            // btnSyncRead
            // 
            btnSyncRead.Location = new Point(396, 255);
            btnSyncRead.Margin = new Padding(5, 4, 5, 4);
            btnSyncRead.Name = "btnSyncRead";
            btnSyncRead.Size = new Size(185, 32);
            btnSyncRead.TabIndex = 15;
            btnSyncRead.Text = "同步读";
            btnSyncRead.UseVisualStyleBackColor = true;
            btnSyncRead.Click += btnSyncRead_Click;
            // 
            // btnAsyncRead
            // 
            btnAsyncRead.Location = new Point(396, 297);
            btnAsyncRead.Margin = new Padding(5, 4, 5, 4);
            btnAsyncRead.Name = "btnAsyncRead";
            btnAsyncRead.Size = new Size(185, 32);
            btnAsyncRead.TabIndex = 16;
            btnAsyncRead.Text = "异步读";
            btnAsyncRead.UseVisualStyleBackColor = true;
            btnAsyncRead.Click += btnAsyncRead_Click;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(16, 78);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(119, 24);
            label4.TabIndex = 17;
            label4.Text = "节点ID标识：";
            label4.TextAlign = ContentAlignment.BottomCenter;
            // 
            // btnSubscription
            // 
            btnSubscription.Location = new Point(396, 339);
            btnSubscription.Margin = new Padding(5, 4, 5, 4);
            btnSubscription.Name = "btnSubscription";
            btnSubscription.Size = new Size(185, 32);
            btnSubscription.TabIndex = 18;
            btnSubscription.Text = "订阅节点";
            btnSubscription.UseVisualStyleBackColor = true;
            btnSubscription.Click += btnSubscription_Click;
            // 
            // btnCloseMo
            // 
            btnCloseMo.Location = new Point(396, 381);
            btnCloseMo.Margin = new Padding(5, 4, 5, 4);
            btnCloseMo.Name = "btnCloseMo";
            btnCloseMo.Size = new Size(185, 32);
            btnCloseMo.TabIndex = 19;
            btnCloseMo.Text = "取消全部订阅";
            btnCloseMo.UseVisualStyleBackColor = true;
            btnCloseMo.Click += btnCloseMo_Click;
            // 
            // btnWrite
            // 
            btnWrite.Location = new Point(396, 423);
            btnWrite.Margin = new Padding(5, 4, 5, 4);
            btnWrite.Name = "btnWrite";
            btnWrite.Size = new Size(185, 32);
            btnWrite.TabIndex = 20;
            btnWrite.Text = "同时写入";
            btnWrite.UseVisualStyleBackColor = true;
            btnWrite.Click += btnWrite_Click;
            // 
            // btnSlmWriter
            // 
            btnSlmWriter.Location = new Point(830, 120);
            btnSlmWriter.Margin = new Padding(5, 4, 5, 4);
            btnSlmWriter.Name = "btnSlmWriter";
            btnSlmWriter.Size = new Size(142, 32);
            btnSlmWriter.TabIndex = 21;
            btnSlmWriter.Text = "单次写入";
            btnSlmWriter.UseVisualStyleBackColor = true;
            btnSlmWriter.Click += btnSlmWriter_Click;
            // 
            // btnNodeAtrr
            // 
            btnNodeAtrr.Location = new Point(396, 213);
            btnNodeAtrr.Margin = new Padding(5, 4, 5, 4);
            btnNodeAtrr.Name = "btnNodeAtrr";
            btnNodeAtrr.Size = new Size(185, 32);
            btnNodeAtrr.TabIndex = 22;
            btnNodeAtrr.Text = "获取节点属性";
            btnNodeAtrr.UseVisualStyleBackColor = true;
            btnNodeAtrr.Click += btnNodeAtrr_Click;
            // 
            // txtLog
            // 
            txtLog.Location = new Point(591, 583);
            txtLog.Margin = new Padding(5, 4, 5, 4);
            txtLog.Name = "txtLog";
            txtLog.Size = new Size(592, 200);
            txtLog.TabIndex = 25;
            txtLog.Text = "";
            // 
            // butRead
            // 
            butRead.Location = new Point(985, 70);
            butRead.Margin = new Padding(5, 4, 5, 4);
            butRead.Name = "butRead";
            butRead.Size = new Size(118, 32);
            butRead.TabIndex = 26;
            butRead.Text = "读取节点";
            butRead.UseVisualStyleBackColor = true;
            butRead.Click += butRead_Click;
            // 
            // button1
            // 
            button1.Location = new Point(985, 116);
            button1.Margin = new Padding(5, 4, 5, 4);
            button1.Name = "button1";
            button1.Size = new Size(118, 32);
            button1.TabIndex = 28;
            button1.Text = "button1";
            button1.UseVisualStyleBackColor = true;
            button1.Click += button1_Click_1;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(16, 124);
            label6.Margin = new Padding(5, 0, 5, 0);
            label6.Name = "label6";
            label6.Size = new Size(100, 24);
            label6.TabIndex = 24;
            label6.Text = "写入数据：";
            // 
            // NodeTree
            // 
            NodeTree.FormattingEnabled = true;
            NodeTree.Location = new Point(16, 213);
            NodeTree.Margin = new Padding(5, 4, 5, 4);
            NodeTree.Name = "NodeTree";
            NodeTree.Size = new Size(370, 571);
            NodeTree.TabIndex = 8;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 185);
            label5.Name = "label5";
            label5.Size = new Size(82, 24);
            label5.TabIndex = 29;
            label5.Text = "节点信息";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(591, 185);
            label7.Name = "label7";
            label7.Size = new Size(82, 24);
            label7.TabIndex = 30;
            label7.Text = "数据信息";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(585, 555);
            label8.Name = "label8";
            label8.Size = new Size(82, 24);
            label8.TabIndex = 31;
            label8.Text = "日志信息";
            // 
            // Main
            // 
            AutoScaleDimensions = new SizeF(11F, 24F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1251, 803);
            Controls.Add(label8);
            Controls.Add(label7);
            Controls.Add(label5);
            Controls.Add(NodeTree);
            Controls.Add(button1);
            Controls.Add(butRead);
            Controls.Add(txtLog);
            Controls.Add(label6);
            Controls.Add(btnNodeAtrr);
            Controls.Add(btnSlmWriter);
            Controls.Add(btnWrite);
            Controls.Add(btnCloseMo);
            Controls.Add(btnSubscription);
            Controls.Add(label4);
            Controls.Add(btnAsyncRead);
            Controls.Add(btnSyncRead);
            Controls.Add(textBox3);
            Controls.Add(txtTargetNode);
            Controls.Add(txtDataInfo);
            Controls.Add(btn_GetNodes);
            Controls.Add(btn_Connect);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtPwd);
            Controls.Add(txtUser);
            Controls.Add(txtAddress);
            Margin = new Padding(5, 4, 5, 4);
            Name = "Main";
            Text = "OPC_UA测试客户端";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtAddress;
        private TextBox txtUser;
        private TextBox txtPwd;
        private Label label1;
        private Label label2;
        private Label label3;
        private Button btn_Connect;
        private Button btn_GetNodes;
        private RichTextBox txtDataInfo;
        private TextBox txtTargetNode;
        private TextBox textBox3;
        private Button btnSyncRead;
        private Button btnAsyncRead;
        private Label label4;
        private Button btnSubscription;
        private Button btnCloseMo;
        private Button btnWrite;
        private Button btnSlmWriter;
        private Button btnNodeAtrr;
        private RichTextBox txtLog;
        private Button butRead;
        private Button button1;
        private Label label6;
        private CheckedListBox NodeTree;
        private Label label5;
        private Label label7;
        private Label label8;
    }
}