using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Encrypt
{
	public partial class MainWindow : Window
	{
		private string[] bandCampCodes = new string[] { "r3hx - yvwn", "adqs - u9de", "khr5 - b4wu", "kelb - xmcp", "u8sm - egs2", "mz4f - ubhc", "fypx - exqw", "wu9h - u5ys", "cmys - 5f2l", "crke - bpxj", "sx3g - wnay", "pcvz - cefr", "8sbz - xbel", "xfj6 - c59h", "h5bu - w3gv", "hrad - xsn9", "qlfe - js5z", "9cab - yd2g", "yhfm - blwu", "yqdf - xjcp", "384v - eys2", "aks7 - u3hc", "ryjd - 54qw", "d2gc - 7pys", "7nhw - ymdn", "vwe4 - 7zwe", "fj64 - 56l3", "mf3h - byqs", "faxj - wu6d", "w7q8 - wp9h", "cmr3 - crgv", "crmw - xdn9", "pxrq - jd5z", "8cmf - yc2g", "tsrv - bhwu", "j4mk - w57r", "6cbt - xrpl", "nhx3 - js7e", "lqc5 - kdpu", "q8hf - 6l3p", "bucl - yjdn", "5wjh - 76we", "bpgh - 5kl3", "a72s - 38qs", "r5le - cf6d", "drpg - wa9h", "7zql - eljw", "x384 - uets", "4vzh - e73l", "4ftj - 35lj", "4du8 - cu5y", "eh5y - cp7r", "gc7y - x2pl", "3swn - jd7e", "dqhm - kcpu", "pgc7 - 6h3p", "jx7j - vwaz", "64mr - gw3g", "kcr5 - b4du", "zjlb - xmsp", "n9sm - etc2", "mt4f - ub4c", "bkpx - ehjw", "w39h - u5ts", "cays - eu3l", "sfke - bplj", "sw3g - wf5y", "v7u4 - btjs", "7mwq - w79d", "xrj6 - ce6h", "4vbu - w38v", "4fad - hwu9", "qdfe - jsaz", "ghab - ys3g", "ycfm - bldu", "ypdf - xjsp", "294v - egc2", "ays7 - u34c", "xjha - 6weu", "j9cb - 642p", "72hw - yqwn", "mxe4 - 7zde", "fe64 - 5zx3", "mr3h - bgjs", "bmxj - wu9d", "wbq8 - c56h", "car3 - cr8v", "sfmw - xsu9", "pwrq - jdaz", "h7zq - 3axj", "hl26 - c7ay", "jsmk - w5fr", "9hbt - h3el", "ncx3 - jsfe", "lpc5 - kseu", "e9hf - 6l2p", "b3cl - ymwn", "axjh - 76de", "rqgh - 56x3", "ab2s - byjs" };

		private string missedCode = "";

		private const int ASCII_RANGE = 94;

		private const int ASCII_START = 32;

		private string cryptResult = "";

		private bool isEncoded;

		public MainWindow()
		{
			this.InitializeComponent();
			this.initLoadingAnimation();
		}

		private string AddSugar(string code)
		{
			int num = this.AsciiSum(code) % 94;
			int num1 = 126 - num;
			string str = ((char)(32 + num)).ToString();
			char chr = (char)num1;
			return string.Concat(str, code, chr.ToString());
		}

		private int AsciiSum(string code)
		{
			int num = 0;
			int length = code.Length;
			byte[] bytes = Encoding.ASCII.GetBytes(code.ToCharArray());
			for (int i = 0; i < length; i++)
			{
				num += bytes[i];
			}
			return num;
		}

		private async Task Crypt(string code)
		{
			Random random = new Random();
			await Task.Delay(5000 + random.Next(0, 5000));
			if (this.missedCode != "")
			{
				code = this.missedCode;
			}
			else if (random.Next(0, 10) == 0)
			{
				await Task.Delay(10000 + random.Next(0, 10000));
				this.codeInTextBox.Foreground = Brushes.Red;
				this.codeInTextBox.IsReadOnly = true;
				int num = random.Next(0, (int)this.bandCampCodes.Length - 1);
				this.cryptResult = this.bandCampCodes[num];
				this.missedCode = code;
				return;
			}
			this.codeInTextBox.IsReadOnly = false;
			this.codeInTextBox.Foreground = Brushes.Black;
			this.missedCode = "";
			List<Func<string, string>> funcs = new List<Func<string, string>>()
			{
				new Func<string, string>(this.TransJunkLetters),
				new Func<string, string>(this.TransCharByPos),
				new Func<string, string>(this.TransOppositeLetters),
				new Func<string, string>(this.TransPalindrome),
				new Func<string, string>(this.TransFibSwap),
				new Func<string, string>(this.TransJunkLetters),
				new Func<string, string>(this.TransCharByPos)
			};
			List<Func<string, string>> funcs1 = funcs;
			int count = funcs1.Count;
			string item = code;
			this.isEncoded = this.IsEncoded(code);
			if (!this.isEncoded)
			{
				for (int i = 0; i < count; i++)
				{
					item = funcs1[i](item);
				}
				item = this.AddSugar(item);
			}
			else
			{
				item = this.RemoveSugar(item);
				for (int j = 0; j < count; j++)
				{
					item = funcs1[count - (j + 1)](item);
				}
			}
			this.cryptResult = item;
		}

		private async void cryptButton_Click(object sender, RoutedEventArgs e)
		{
			this.loadingAnimation.start();
			this.cryptButton.Visibility = System.Windows.Visibility.Hidden;
			this.codeInTextBox.Visibility = System.Windows.Visibility.Hidden;
			await this.Crypt(this.codeInTextBox.Text);
			this.loadingAnimation.stop();
			this.codeInTextBox.Text = this.cryptResult;
			this.cryptResult = "";
			this.cryptButton.Visibility = System.Windows.Visibility.Visible;
			this.codeInTextBox.Visibility = System.Windows.Visibility.Visible;
		}

		private void initLoadingAnimation()
		{
			List<BitmapImage> bitmapImages = new List<BitmapImage>();
			for (int i = 0; i < 47; i++)
			{
				bitmapImages.Add(new BitmapImage(new Uri(string.Concat("pack://application:,,,/images/loading/loading", string.Format("{0:D5}", i), ".png"))));
			}
			this.loadingAnimation.Images = bitmapImages;
			this.loadingAnimation.Initiate();
		}

		private bool IsEncoded(string code)
		{
			if (code.Length < 2)
			{
				return false;
			}
			char chr = code[0];
			int num = code[code.Length - 1];
			string str = this.RemoveSugar(code);
			int num1 = this.AsciiSum(str) % 94;
			if (chr != (char)(32 + num1))
			{
				return false;
			}
			return num == 126 - num1;
		}

		private string RemoveSugar(string code)
		{
			char[] charArray = code.ToCharArray();
			return new string(charArray.Skip<char>(1).Take<char>((int)charArray.Length - 2).ToArray<char>());
		}

		private string TransCharByPos(string code)
		{
			string str = (code.Length > 0 ? code[0].ToString() ?? "" : "");
			Random random = new Random((code.Length > 0 ? (int)code[0] : 0));
			if (!this.isEncoded)
			{
				for (int i = 1; i < code.Length; i++)
				{
					char chr = (char)(32 + (random.Next(0, 94) + code[i] - 32) % 94);
					str = string.Concat(str, chr.ToString());
				}
			}
			else
			{
				for (int j = 1; j < code.Length; j++)
				{
					char chr1 = code[j];
					char chr2 = (char)(32 + (code[j] - 32 + 94 - (char)random.Next(0, 94)) % 94);
					str = string.Concat(str, chr2.ToString());
				}
			}
			return str;
		}

		private string TransFibSwap(string code)
		{
			int length = code.Length;
			int num = 1;
			int num1 = 2;
			char[] charArray = code.ToCharArray();
			while (num1 < length - 1)
			{
				char chr = charArray[num];
				charArray[num] = charArray[num1];
				charArray[num1] = chr;
				num += num1;
				num1 += num;
			}
			return new string(charArray);
		}

		private string TransJunkLetters(string code)
		{
			Random random = new Random((code.Length > 0 ? (int)code[code.Length - 1] : 0));
			string str = code;
			if (!this.isEncoded)
			{
				Random random1 = new Random((code.Length > 0 ? (int)code[0] : 0));
				for (int i = 0; i < str.Length - 1; i++)
				{
					int num = str[i] % (char)random.Next(1, 5);
					string str1 = "";
					for (int j = 0; j < num; j++)
					{
						char chr = (char)random1.Next(32, 126);
						str1 = string.Concat(str1, chr.ToString());
					}
					str = string.Concat(str.Substring(0, i + 1), str1, str.Substring(i + 1));
					i += num;
				}
			}
			else
			{
				for (int k = 0; k < str.Length - 1; k++)
				{
					int num1 = str[k] % (char)random.Next(1, 5);
					str = string.Concat(str.Substring(0, k + 1), str.Substring(k + 1 + num1));
				}
			}
			return str;
		}

		private string TransOppositeLetters(string code)
		{
			int length = code.Length;
			string str = "";
			for (int i = 0; i < length; i++)
			{
				int num = code[i];
				int num1 = 32 + (num - 32 + 47) % 94;
				char chr = (char)num1;
				str = string.Concat(str, chr.ToString());
			}
			return str;
		}

		private string TransPalindrome(string code)
		{
			int length = code.Length;
			string str = "";
			for (int i = 0; i < length; i++)
			{
				char chr = code[length - (i + 1)];
				str = string.Concat(str, chr.ToString());
			}
			return str;
		}
	}
}