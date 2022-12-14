using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.IO.Ports;
using System.Threading;
using System.Management;

namespace Design_performance
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private SerialPort _serialPort;
		// Создание потока
		Thread Com_Refresher;		
		bool To_Work = true;
		// Переменная для предыдущих портов
		string[] Last_Ports = new string[0];
		public MainWindow()
		{
			InitializeComponent();


			// Инициализация
			InitializeComponent();
			// Задача
			Com_Refresher = new Thread(Refresh_Com_Ports);
			// Запуск
			Com_Refresher.Start();

			string[] myPort; //Массив
			myPort = System.IO.Ports.SerialPort.GetPortNames(); // Занесение доступных портов в массив
			if (string.IsNullOrWhiteSpace(comboBox_Port.Text))
			{
				foreach (string port in myPort)
				{
					comboBox_Port.Items.Add(port); //Добавление списка портов в comboBox			
				}
			}

			//Компоненты
			_serialPort = new SerialPort();
			_serialPort.PortName = "COM1";
			_serialPort.BaudRate = 4800;
			_serialPort.DtrEnable = true;

		}
		
		public string GetComPortInformation(string name)
		{
			//Вывод полной информации о портах
			ManagementObjectCollection mbsList = null;
			StringBuilder sb = new StringBuilder(2000);

			ManagementObjectSearcher mbs = new ManagementObjectSearcher(
			"SELECT * FROM Win32_SerialPort WHERE DeviceID = '" + name + "'"
			);

			using (mbs)
			{
				mbsList = mbs.Get();

				foreach (ManagementObject mo in mbsList)
				{
					object val = mo["Name"];
					if (val != null) sb.AppendLine(val.ToString());

					foreach (var p in mo.Properties)
					{
						sb.Append("* " + p.Name + ": ");
						if (p.Value != null)
						{
							sb.Append(p.Value.ToString());
						}
						else sb.Append("null");
						sb.AppendLine();
					}
					break;
				}
				return sb.ToString();
			}
		}
	private void Refresh_Com_Ports()// Обновление порта
									
		{
						
		}
		private bool Is_Arrays_Equeals(string[] First, string[] Second)
		// Проверка массивов на идентичность
		{
			// Переменная для совпадения портов
			bool Is_Equeals = true;
			// Если порты одинаковых размеров
			if (First.Length == Second.Length)
			{
				// Для всех портов
				for (int i = 0; i < First.Length; i++)
				{
					// Если порты не совпадают
					if (First[i] != Second[i])
					{
						// Порты не совпадают
						Is_Equeals = false;
					}
				}
			}
			// Если порты не одного размера
			else
			{
				// Порты не совпадают
				Is_Equeals = false;
			}
			// Вернуть ответ
			return Is_Equeals;
		}
		private bool IsMaximized = false;
		private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{
			//Окно
			if (e.ClickCount == 2)
			{
				if (IsMaximized)
				{
					this.WindowState = WindowState.Normal;
					this.Width = 800;
					this.Height = 450;

					IsMaximized = false;
				}
				else
				{
					this.WindowState = WindowState.Maximized;

					IsMaximized = false;
				}
			}
		}
		private void but_Port_Click(object sender, RoutedEventArgs e)
		{
			_serialPort.BaudRate = 4800;
			_serialPort.PortName = comboBox_Port.Text.ToString();
			if (_serialPort.IsOpen == false)
			{
				_serialPort.Open();
				
				textBlock_status.Text = "Порт подключен";
				MessageBox.Show("Порт подключен");
			}
			else
			{
				MessageBox.Show("Порт закрыт");
			}
			Exit_Port.Visibility = Visibility.Visible;
			but_Port.Visibility = Visibility.Hidden;
		}
		private void but_InfoPort_Click(object sender, RoutedEventArgs e)
		{
			//Вывод на клик полной информации
			string com_port = comboBox_Port.Text;
			txt_new.Text = GetComPortInformation(com_port);
		}

		private void comboBox_Port_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{

			
			// Найденные порты
			ManagementObjectCollection Ports;
			// Поисковик данных
			ManagementObjectSearcher Ports_Found;
			// Поиск портов по запросу
			Ports_Found = new ManagementObjectSearcher("Select * from Win32_SerialPort");
			// Полученные данные
			Ports = Ports_Found.Get();
			// Для всех полученных данных
			foreach (ManagementObject Port in Ports)
			{
				// Информация для вывода данных о портах
				Inf_Text.Text = Port["DeviceID"].ToString() + "\r\n";
				Inf_Text2.Text += Port["PNPDeviceID"].ToString() + "\r\n";
				Inf_Text3.Text += Port["Name"].ToString() + "\r\n";
				Inf_Text4.Text += Port["Caption"].ToString() + "\r\n";
				Inf_Text5.Text += Port["Description"].ToString() + "\r\n";
				Inf_Text6.Text += Port["ProviderType"].ToString() + "\r\n";
				Inf_Text7.Text += Port["Status"].ToString();
			}			
		}
		private void Border_MouseDown(object sender, MouseButtonEventArgs e)
		{
			if (e.ChangedButton == MouseButton.Left) //Окно
			{
				this.DragMove();
			}
		}
		private void Exit_Port_Click(object sender, RoutedEventArgs e)
		{
			//Отключение порта
			if (_serialPort.IsOpen == true)
				_serialPort.Close();
			textBlock_status.Text = "Порт отключен";
			but_Port.Visibility = Visibility.Visible;
			MessageBox.Show("Порт отключен");
			Exit_Port.Visibility = Visibility.Hidden;
		}
	}
}

