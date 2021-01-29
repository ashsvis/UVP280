using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace UVP280
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Чтение данных из прибора УВП-280.01\n");
            IPAddress address;
            int np;
            while (true)
            {
                Console.Write("Введите IP преобразователя: ");
                var ip = Console.ReadLine();
                if (IPAddress.TryParse(ip, out address))
                {
                    while (true)
                    {
                        Console.Write("Введите номер трубопровода: ");
                        var tp = Console.ReadLine();
                        if (int.TryParse(tp, out np) && np > 0 && np <= 2)
                            goto trm;
                    }
                }
            }
            trm: Console.Clear();
            while (true)
            {
                Console.CursorLeft = 0;
                Console.CursorTop = 0;
                Console.WriteLine("Чтение данных из прибора УВП-280.01\n");
                FetchData(address, np);
                Console.WriteLine("\nНажмите ^C для завершения работы...");
            }
        }

        private static void FetchData(IPAddress ip, int trd)
        {
            using (var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                socket.SendTimeout = 90000;
                socket.ReceiveTimeout = 90000;
                var remoteEp = new IPEndPoint(ip, 502); //"192.168.0.56"
                socket.Connect(remoteEp);
                if (socket.Connected)
                {
                    Console.WriteLine("Контрольные константы:");
                    FetchValue(socket, 1, ModbusFunc.H, 110, DataType.Long, "DCBA", "Контрольная константа (1234567890)", "int32");
                    FetchValue(socket, 1, ModbusFunc.H, 112, DataType.Single, "DCBA", "Контрольная константа (123.4567)", "float");
                    FetchValue(socket, 1, ModbusFunc.H, 114, DataType.Double, "HGFEDCBA", "Контрольная константа (123.4567890123456)", "double");
                    Console.WriteLine($"\nТекущие параметры трубопровода № {trd}:");
                    var @base = 2000 + (trd - 1) * 100;
                    FetchValue(socket, 1, ModbusFunc.R, @base + 0, DataType.Single, "DCBA", "Избыточное давление", "кгс/см2", 98067);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 2, DataType.Single, "DCBA", "Абсолютное давление", "Па");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 4, DataType.Single, "DCBA", "Температура", "°C");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 6, DataType.Single, "DCBA", "Энтальпия", "Дж/кг");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 8, DataType.Single, "DCBA", "Массовый расход", "кг/с");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 10, DataType.Single, "DCBA", "Масса", "кг");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 12, DataType.Single, "DCBA", "Объём в рабочих условиях", "тыс.м3(р.у)", 1000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 14, DataType.Single, "DCBA", "Объём в стандартных условиях", "тыс.м3(ст.у)", 1000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 16, DataType.Single, "DCBA", "Тепловая мощность", "МВт", 1000000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 18, DataType.Single, "DCBA", "Тепловая энергия", "МДж", 1000000);
                    //
                    FetchValue(socket, 1, ModbusFunc.R, @base + 20, DataType.Long, "DCBA", "Время штатной работы", "часов", 3600);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 22, DataType.Long, "DCBA", "Время нештатных ситуаций", "часов", 3600);
                    //
                    FetchValue(socket, 1, ModbusFunc.R, @base + 24, DataType.Single, "DCBA", "Параметр (масс. расход)", "кг/с");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 26, DataType.Single, "DCBA", "Параметр (объёмный расход в р.у.)", "м3(р.у)/с");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 28, DataType.Single, "DCBA", "Параметр (объёмный расход в ст.у.)", "м3(ст.у)/с");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 30, DataType.Single, "DCBA", "Параметр (масса)", "кг");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 32, DataType.Single, "DCBA", "Параметр (объём в р.у.)", "м3(р.у)");
                    //
                    FetchValue(socket, 1, ModbusFunc.R, @base + 34, DataType.Integer, "BA", "Количество ошибок на трубопроводе");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 35, DataType.Integer, "BA", "Уровень критичности ошибок трубопровода");
                    //
                    FetchValue(socket, 1, ModbusFunc.R, @base + 36, DataType.Single, "DCBA", "Плотность среды в ст.у.", "кг/м3(ст.у)");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 38, DataType.Single, "DCBA", "Плотность измеряемой среды в р.у.", "кг/м3(р.у)");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 40, DataType.Single, "DCBA", "Перепад давления для трубопроводов на СУ", "Па");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 42, DataType.Single, "DCBA", "Объемный расход в ст.у.", "м3(ст.у)/с");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 44, DataType.Single, "DCBA", "Объемный расход в р.у.", "м3(р.у)/с");
                    //
                    Console.WriteLine("\nПоказатели за предыдущий час:");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 60, DataType.Single, "DCBA", "Масса", "тонн", 1000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 62, DataType.Single, "DCBA", "Объём в рабочих условиях", "тыс.м3(р.у)", 1000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 64, DataType.Single, "DCBA", "Объём в стандартных условиях", "тыс.м3(ст.у)", 1000);
                    //FetchValue(socket, 1, ModbusFunc.IR, @base + 66, DataType.Single, "DCBA", "Параметр датчика расхода/количества (масса)");
                    //FetchValue(socket, 1, ModbusFunc.IR, @base + 68, DataType.Single, "DCBA", "Параметр датчика расхода/количества (объём в р.у.)");
                    //
                    Console.WriteLine("\nПоказатели за предыдущие сутки:");
                    FetchValue(socket, 1, ModbusFunc.R, @base + 80, DataType.Single, "DCBA", "Масса", "тонн", 1000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 82, DataType.Single, "DCBA", "Объём в рабочих условиях", "тыс.м3(р.у)", 1000);
                    FetchValue(socket, 1, ModbusFunc.R, @base + 84, DataType.Single, "DCBA", "Объём в стандартных условиях", "тыс.м3(ст.у)", 1000);
                    //FetchValue(socket, 1, ModbusFunc.IR, @base + 86, DataType.Single, "DCBA", "Параметр датчика расхода/количества (масса)");
                    //FetchValue(socket, 1, ModbusFunc.IR, @base + 88, DataType.Single, "DCBA", "Параметр датчика расхода/количества (объём в р.у.)");
                }
            }
        }

        private static void FetchValue(Socket socket, byte node, ModbusFunc func, int addr, DataType ntype, string nswap, string desc, string eu = null, double divider = 1.0)
        {
            socket.Send(PrepareFetchParam(node, func, addr, ntype));
            Thread.Sleep(150);
            var buff = new byte[1024];
            var numBytes = socket.Receive(buff);
            if (numBytes > 0)
            {
                var answer = CleanAnswer(buff);
                if (CheckAnswer(answer, node, func, ntype))
                {
                    var result = EncodeFetchAnswer(answer, node, func, addr, ntype, nswap);
                    result.Desc = desc;
                    result.Unit = eu;
                    if (divider != 1.0)
                        result.Value = Math.Round(Convert.ToDouble(result.Value) / divider, 8);
                    Console.WriteLine(result);
                }
            }
        }

        private static AnswerData EncodeFetchAnswer(byte[] answer, byte node, ModbusFunc func, int regAddr, DataType typeValue, string typeSwap, string unitValue = null)
        {
            var dataset = new List<byte>(); // содержит данные ответа
            object value = string.Empty;
            switch (typeValue)
            {
                case DataType.Integer:
                    if (answer.Length == 5)
                    {
                        var data = BitConverter.ToUInt16(Swap(answer, 3, typeSwap), 0);
                        if (unitValue == "bits")
                        {
                            var sb = new StringBuilder();
                            for (var i = 0; i < 16; i++)
                            {
                                var bc = data & 0x01;
                                if (bc > 0)
                                    sb.Insert(0, "1");
                                else
                                    sb.Insert(0, "0");
                                data = (UInt16)(data >> 1);
                                if (i % 4 == 3)
                                    sb.Insert(0, " ");
                            }
                            value = sb.ToString().Trim();
                        }
                        else
                            value = data;
                    }
                    break;
                case DataType.Long:
                    if (answer.Length == 7)
                    {
                        var data = BitConverter.ToUInt32(Swap(answer, 3, typeSwap), 0);
                        if (unitValue == "UTC")
                        {
                            var dateTime = ConvertFromUnixTimestamp(data).ToLocalTime();
                            value = dateTime; //dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.GetCultureInfo("en-US"));
                        }
                        else
                            value = data;
                    }
                    break;
                case DataType.Single:
                    if (answer.Length == 7)
                    {
                        var data = BitConverter.ToSingle(Swap(answer, 3, typeSwap), 0);
                        value = data;
                    }
                    break;
                case DataType.Double:
                    if (answer.Length == 11)
                    {
                        var data = BitConverter.ToDouble(Swap(answer, 3, typeSwap), 0);
                        value = data;
                    }
                    break;
            }
            return new AnswerData()
            {
                Node = node,
                Func = func,
                Addr = regAddr,
                Value = value
            };
        }

        private static DateTime ConvertFromUnixTimestamp(uint timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        private static byte[] Swap(byte[] buff, int startIndex, string typeSwap)
        {
            var list = buff.Skip(startIndex).ToArray();
            if (list.Length == 2)
            {
                switch (typeSwap)
                {
                    case "AB":
                        return new byte[] { list[0], list[1] };
                    case "BA":
                        return new byte[] { list[1], list[0] };
                    default:
                        return list;
                }
            }
            else if (list.Length == 4)
            {
                switch (typeSwap)
                {
                    case "ABCD":
                        return new byte[] { list[0], list[1], list[2], list[3] };
                    case "CDAB":
                        return new byte[] { list[2], list[3], list[0], list[1] };
                    case "BADC":
                        return new byte[] { list[1], list[0], list[3], list[2] };
                    case "DCBA":
                        return new byte[] { list[3], list[2], list[1], list[0] };
                    default:
                        return list;
                }
            }
            else if (list.Length == 8)
            {
                switch (typeSwap)
                {
                    case "ABCDEFGH":
                        return new byte[] { list[0], list[1], list[2], list[3], list[4], list[5], list[6], list[7] };
                    case "GHEFCDAB":
                        return new byte[] { list[6], list[7], list[4], list[5], list[2], list[3], list[0], list[1] };
                    case "BADCFEHG":
                        return new byte[] { list[1], list[0], list[3], list[2], list[5], list[4], list[7], list[6] };
                    case "HGFEDCBA":
                        return new byte[] { list[7], list[6], list[5], list[4], list[3], list[2], list[1], list[0] };
                    default:
                        return list;
                }
            }
            else
                return list;
        }

        private static bool CheckAnswer(byte[] answer, byte node, ModbusFunc func, DataType typeValue)
        {
            var datacount = DataLength(typeValue);
            if (datacount * 2 + 3 == answer.Length)
            {
                if (answer[0] == node && answer[1] == (byte)func && datacount * 2 == answer[2])
                    return true;
            }
            return false;
        }

        private static byte[] CleanAnswer(IEnumerable<byte> receivedBytes)
        {
            var source = new List<byte>();
            var length = 0;
            var n = 0;
            foreach (var b in receivedBytes)
            {
                if (n == 5)
                    length = b;
                else if (n > 5 && length > 0)
                {
                    source.Add(b);
                    if (source.Count == length)
                        break;
                }
                n++;
            }
            return source.ToArray();
        }

        private static byte[] PrepareFetchParam(byte node, ModbusFunc func, int addr, DataType typeValue)
        {
            var datacount = DataLength(typeValue);
            return EncodeData(0, 0, 0, 0, 0, 6, (byte)node, (byte)func,
                                       (byte)(addr >> 8), (byte)(addr & 0xff),
                                       (byte)(datacount >> 8), (byte)(datacount & 0xff));
        }

        private static int DataLength(DataType typeValue)
        {
            int datacount = 1; // запрашиваем количество регистров
            switch (typeValue)
            {
                case DataType.Integer:
                    datacount = 1;
                    break;
                case DataType.Long:
                case DataType.Single:
                    datacount = 2;
                    break;
                case DataType.Double:
                    datacount = 4;
                    break;
            }
            return datacount;
        }

        private static byte[] EncodeData(params byte[] list)
        {
            var result = new byte[list.Length];
            for (var i = 0; i < list.Length; i++) result[i] = list[i];
            return result;
        }
    }

    public enum ModbusFunc
    {
        H = 3,
        R = 4
    }

    public enum DataType
    {
        Integer,
        Long,
        Single,
        Double
    }

    public class AnswerData
    {
        public byte Node { get; set; }          // байт адреса прибора
        public ModbusFunc Func { get; set; }    // номер функции Modbus
        public int Addr { get; set; }           // номер регистра
        public string Desc { get; set; }
        public object Value { get; set; }
        public string Unit { get; set; }

        public override string ToString()
        {
            return string.Format("{0}{1} {4,-42} {2,15} {3}", Func, Addr, Value, Unit, Desc);
        }
    }
}
