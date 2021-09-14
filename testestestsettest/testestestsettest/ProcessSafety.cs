﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace testestestsettest
{
    public partial class ProcessSafety : Form
    {
        private System.Data.SqlClient.SqlConnection Connect = null;
        private string strConn = "Data Source=61.105.9.203; Initial Catalog=AppDev;User ID=spa;Password=spiral_0904";

        public ProcessSafety()
        {
            InitializeComponent();
            //serialPort1.Open();
        }

        private void ProcessSafety_Load(object sender, EventArgs e)
        {
            //try
            //{
            //    //Sql 커넥션
            //    //Sql 커넥션에 등록 및 객체 선언
            //    Connect = new SqlConnection(Common.DbPath);
            //    Connect.Open();

            //    if (Connect.State != System.Data.ConnectionState.Open)
            //    {
            //        MessageBox.Show("데이터 베이스 연결에 실패 하였습니다.");
            //        return;
            //    }

            //    SqlDataAdapter adapter = new SqlDataAdapter("SELECT DISTINCT NO,CO2,GAS,LIGHT,FLAME,MAKEDATE,CHECKFLAG,CHECKDATE,MAKER FROM TB_ENVIROMENTSTATE", Connect);
            //    DataTable dtTemp = new DataTable();
            //    adapter.Fill(dtTemp);

            //    if (dtTemp.Rows.Count == 0)
            //    {
            //        grid1.DataSource = null;
            //        return;
            //    }
            //    grid1.DataSource = dtTemp;   //데이터 그리드 뷰에 데이터 테이블 등록

            //    //그리드뷰의 헤더 명칭 선언
            //    grid1.Columns["NO"].HeaderText = "no";
            //    grid1.Columns["CO2"].HeaderText = "이산화탄소";
            //    grid1.Columns["GAS"].HeaderText = "가스누출";
            //    grid1.Columns["LIGHT"].HeaderText = "조도";
            //    grid1.Columns["FLAME"].HeaderText = "불꽃";
            //    grid1.Columns["MAKEDATE"].HeaderText = "발생시간";


            //    // 그리드 뷰의 폭 지정
            //    grid1.Columns[0].Width = 150;
            //    grid1.Columns[1].Width = 150;
            //    grid1.Columns[2].Width = 150;
            //    grid1.Columns[3].Width = 150;
            //    grid1.Columns[4].Width = 150;
            //    grid1.Columns[5].Width = 150;


            //    //컬럼의 수정 여부를 지정 한다
            //    grid1.Columns["NO"].ReadOnly = true;    //기본키라 수정하면 안됌, 단 신규로 추가될때는 해야함
            //    grid1.Columns["CO2"].ReadOnly = true;
            //    grid1.Columns["GAS"].ReadOnly = true;
            //    grid1.Columns["LIGHT"].ReadOnly = true;
            //    grid1.Columns["FLAME"].ReadOnly = true;
            //    grid1.Columns["MAKEDATE"].ReadOnly = true;

            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.ToString());
            //}
            //finally
            //{
            //    Connect.Close();    //DB 연결 끊어주기
            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("상세보기로 이동합니다");
            ProcessDetail processDetail = new ProcessDetail();
            processDetail.ShowDialog();
        }

        // btn 가동 버튼은 visible false로 수정하기 
        private void btnRun_Click(object sender, EventArgs e)
        {
            //<생각정리>
            //C#의 프로그램상에서 가동버튼을 클릭함 => DB에서 신호를 보냄(sql로 요청함)              => DB의 설비이력에 정지시간 및 이력이 등록됨(Stored procedure에서 update 구문을 작성함) (테이블 설계상 이게 맞을거 같은데?)
            try
            {
                if (grid1.Rows.Count == 0) return;
                string nores = grid1.CurrentRow.Cells["ONOFFFLAG"].Value.ToString();
                if (MessageBox.Show("설비를 가동 하시겠습니까?", "가동", MessageBoxButtons.YesNo)
                    == DialogResult.Yes) return;

                string Num = grid1.CurrentRow.Cells["ONOFFFLAG"].Value.ToString();

                SqlCommand cmd = new SqlCommand();
                SqlTransaction Tran;   //승인을 할지 거절을 할지 권한을 가지겟다

                Connect = new SqlConnection(Common.DbPath);
                Connect.Open();

                Tran = Connect.BeginTransaction("TestTran");
                cmd.Transaction = Tran;
                cmd.Connection = Connect;

                //주석 사유 : 정지버튼과 동일한 사유
                //cmd.CommandText = "INSERT INTO TB_WARNINGMANAGE(custID, resDate,  resState,  roomNum ,NoShow) " +
                //                          "VALUES('" + Common.LogInId + "','" + Date + "','" + "Y" + "','" + Num + "','" + "N" + "')" +
                //                  "INSERT INTO TB_ENVIROMENTSTATE(MAKEDATE,CHECKFLAG,CHECKDATE) " +
                //                          "VALUES('" + Num + "','" + Date + "')";
                cmd.ExecuteNonQuery();

                // 성공 시 DB COMMIT

                if (nores == "가동")
                {
                    Tran.Commit();
                    MessageBox.Show("설비가 가동 되었습니다.");
                }
                else
                {
                    MessageBox.Show("설비를 가동할 수 없습니다.");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Connect.Close();
            }
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (grid1.Rows.Count == 0) return;
                string nores = grid1.CurrentRow.Cells["ONOFFFLAG"].Value.ToString();
                if (MessageBox.Show("설비를 정지 하시겠습니까?", "정지", MessageBoxButtons.YesNo)
                    == DialogResult.No) return;

                string Num = grid1.CurrentRow.Cells["ONOFFFLAG"].Value.ToString();

                SqlCommand cmd = new SqlCommand();
                SqlTransaction Tran;   //승인을 할지 거절을 할지 권한을 가지겟다

                Connect = new SqlConnection(Common.DbPath);
                Connect.Open();

                Tran = Connect.BeginTransaction("TestTran");
                cmd.Transaction = Tran;
                cmd.Connection = Connect;

                //주석 사유 : TRANSACTION으로 걸려야 할 값 및 삽입되어야 할 값에 대해서 DB.Helper 안쓴버전으로 입력하였으나, 추가검토가 필요함.  
                //cmd.CommandText = "INSERT INTO TB_WARNINGMANAGE(custID, resDate,  resState,  roomNum ,NoShow) " +
                //                          "VALUES('" + Common.LogInId + "','" + Date + "','" + "Y" + "','" + Num + "','" + "N" + "')" +
                //                  "INSERT INTO TB_ENVIROMENTSTATE(MAKEDATE,CHECKFLAG,CHECKDATE) " +
                //                          "VALUES('" + Num + "','" + Date + "')";
                cmd.ExecuteNonQuery();

                // 성공 시 DB COMMIT

                if (nores == "정지가능")
                {
                    Tran.Commit();
                    MessageBox.Show("정지되었습니다.");
                }
                else
                {
                    MessageBox.Show("정지할 수 없습니다..");
                }
            }
            catch (Exception ex)
            {

            }
            finally
            {
                Connect.Close();
            }
        }

        // 라즈베리파이 센서연결 참고
        //using System;
        //using System.Collections.Generic;
        //using System.Linq;
        //using System.Text;
        //using System.Threading;
        //using System.Threading.Tasks;
        //using System.Diagnostics;
        //using Windows.Devices.Gpio;

        //namespace RaspberryPi
        //    {
        //        class UcSensor
        //        {
        //            GpioController gpio = GpioController.GetDefault();

        //            GpioPin TriggerPin;
        //            GpioPin EchoPin;

        //            //Contructor
        //            public UcSensor(int TriggerPin, int EchoPin)
        //            {
        //                //Setting up gpio pin's
        //                this.TriggerPin = gpio.OpenPin(TriggerPin);
        //                this.EchoPin = gpio.OpenPin(EchoPin);

        //                this.TriggerPin.SetDriveMode(GpioPinDriveMode.Output);
        //                this.EchoPin.SetDriveMode(GpioPinDriveMode.Input);

        //                this.TriggerPin.Write(GpioPinValue.Low);
        //            }

        //            public double GetDistance()
        //            {
        //                ManualResetEvent mre = new ManualResetEvent(false);
        //                mre.WaitOne(500);

        //                //Send pulse
        //                this.TriggerPin.Write(GpioPinValue.High);
        //                mre.WaitOne(TimeSpan.FromMilliseconds(0.01));
        //                this.TriggerPin.Write(GpioPinValue.Low);

        //                //Recieve pusle
        //                while (this.EchoPin.Read() == GpioPinValue.Low)
        //                {
        //                }
        //                DateTime start = DateTime.Now;

        //                while (this.EchoPin.Read() == GpioPinValue.High)
        //                {
        //                }
        //                DateTime stop = DateTime.Now;

        //                //Calculating distance
        //                double timeBetween = (stop - start).TotalSeconds;
        //                double distance = timeBetween * 17000;

        //                return distance;
        //            }

        //        }
        //    }


    }
}
