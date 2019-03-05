using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;

class Translate {
  private const int ASCII_RANGE = 94;
  private const int ASCII_START = 32;

  private string cryptResult = "";
  private bool isEncoded;

  private int AsciiSum(string code) {
    int num = 0;
    int length = code.Length;
    byte[] bytes = Encoding.ASCII.GetBytes(code.ToCharArray());
    for (int i = 0; i < length; i++) {
      num += bytes[i];
    }
    return num;
  }

  private string AddSugar(string code) {
    int num = this.AsciiSum(code) % 94;
    int num1 = 126 - num;
    string str = ((char)(32 + num)).ToString();
    char chr = (char)num1;
    return string.Concat(str, code, chr.ToString());
  }

  public void Crypt(string code) {
    List<Func<string, string>> funcs = new List<Func<string, string>>() {
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
    if (!this.isEncoded) {
      for (int i = 0; i < count; i++) {
        item = funcs1[i](item);
      }
      item = this.AddSugar(item);
    } else {
      item = this.RemoveSugar(item);
      for (int j = 0; j < count; j++) {
        item = funcs1[count - (j + 1)](item);
      }
    }
    this.cryptResult = item;
  }

  private bool IsEncoded(string code) {
    if (code.Length < 2) {
      return false;
    }
    char chr = code[0];
    int num = code[code.Length - 1];
    string str = this.RemoveSugar(code);
    int num1 = this.AsciiSum(str) % 94;
    if (chr != (char)(32 + num1)) {
      return false;
    }
    return num == 126 - num1;
  }

  private string RemoveSugar(string code) {
    char[] charArray = code.ToCharArray();
    return new string(charArray.Skip<char>(1).Take<char>((int)charArray.Length - 2).ToArray<char>());
  }

  private string TransCharByPos(string code) {
    string str = (code.Length > 0 ? code[0].ToString() ?? "" : "");
    Random random = new Random((code.Length > 0 ? (int)code[0] : 0));
    if (!this.isEncoded) {
      for (int i = 1; i < code.Length; i++) {
        char chr = (char)(32 + (random.Next(0, 94) + code[i] - 32) % 94);
        str = string.Concat(str, chr.ToString());
      }
    } else {
      for (int j = 1; j < code.Length; j++) {
        char chr1 = code[j];
        char chr2 = (char)(32 + (code[j] - 32 + 94 - (char)random.Next(0, 94)) % 94);
        str = string.Concat(str, chr2.ToString());
      }
    }
    return str;
  }

  private string TransFibSwap(string code) {
    int length = code.Length;
    int num = 1;
    int num1 = 2;
    char[] charArray = code.ToCharArray();
    while (num1 < length - 1) {
      char chr = charArray[num];
      charArray[num] = charArray[num1];
      charArray[num1] = chr;
      num += num1;
      num1 += num;
    }
    return new string(charArray);
  }

  private string TransJunkLetters(string code) {
    Random random = new Random((code.Length > 0 ? (int)code[code.Length - 1] : 0));
    string str = code;
    if (!this.isEncoded) {
      Random random1 = new Random((code.Length > 0 ? (int)code[0] : 0));
      for (int i = 0; i < str.Length - 1; i++) {
        int num = str[i] % (char)random.Next(1, 5);
        string str1 = "";
        for (int j = 0; j < num; j++) {
          char chr = (char)random1.Next(32, 126);
          str1 = string.Concat(str1, chr.ToString());
        }
        str = string.Concat(str.Substring(0, i + 1), str1, str.Substring(i + 1));
        i += num;
      }
    } else {
      for (int k = 0; k < str.Length - 1; k++) {
        int num1 = str[k] % (char)random.Next(1, 5);
        str = string.Concat(str.Substring(0, k + 1), str.Substring(k + 1 + num1));
      }
    }
    return str;
  }

  private string TransOppositeLetters(string code) {
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

  private string TransPalindrome(string code) {
    int length = code.Length;
    string str = "";
    for (int i = 0; i < length; i++)
    {
      char chr = code[length - (i + 1)];
      str = string.Concat(str, chr.ToString());
    }
    return str;
  }

  public static void Main (string[] args) {
    Translate c = new Translate();
    string code = "encode me.";
    c.Crypt(code);
    Console.WriteLine(c.cryptResult);
  }
}