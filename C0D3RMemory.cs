using C0D3RMem.Memory;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace C0D3RMem
{
    static class API
	{
		public const uint MEM_COMMIT = 0x1000;
		public const uint PAGE_READONLY = 0x02;
		public const uint PAGE_READWRITE = 0x04;

		public const uint PROCESS_ALL_ACCESS = 0x1F0FFF;
		public const uint PROCESS_VM_READ = (0x010);
		public const uint PROCESS_VM_WRITE = (0x020);
		public const uint PROCESS_VM_OPERATION = (0x08);

		[DllImport("kernel32.dll")]
		public static extern IntPtr OpenProcess(uint dwAccess, bool inherit, int pid);

		[DllImport("kernel32.dll")]
		public static extern bool CloseHandle(IntPtr handle);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool ReadProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesRead);

		[DllImport("kernel32.dll")]
		public static extern bool WriteProcessMemory(IntPtr hProcess, long lpBaseAddress, [In, Out] byte[] lpBuffer, ulong dwSize, out IntPtr lpNumberOfBytesWritten);

		[DllImport("kernel32", SetLastError = true)]
		public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flAllocationType, uint flProtect);

		[DllImport("kernal32.dll")]
		public static extern bool VirtualQueryEx(IntPtr hProcess, IntPtr lpAddress, out MEMORY_BASIC_INFORMATION lpBuffer, uint dwLength);

		[DllImport("kernel32.dll", SetLastError = true)]
		public static extern bool VirtualProtectEx(IntPtr hProcess, IntPtr lpAddress, uint dwSize, uint flNewProtect, out uint lpflOldProtect);

		public static byte[] ConvertStringToBytes(string byteString)
		{
			string[] elements = byteString.Split(" ");
			byte[] bytes = new byte[elements.Length];
			for (int i = 0; i < elements.Length; i++)
			{
				if (elements[i].Contains("?"))
				{
					bytes[i] = 0x0;
				}
				else
				{
					bytes[i] = Convert.ToByte(elements[i], 16);
				}
			}
			return bytes;
		}

		[StructLayout(LayoutKind.Sequential)]
		public struct MEMORY_BASIC_INFORMATION
		{
			public IntPtr BaseAddress;
			public IntPtr AllocationBase;
			public uint AllocationProtect;
			public IntPtr RegionSize;
			public uint State;
			public uint Protect;
			public uint Type;
		}

	}
	public class C0D3RMemory
	{
		private IntPtr ProcessHandle;
		private Process? curProc = null;
		private Reader? _reader = null;
		private Writer? _writer = null;
		private Scanner? _scanner = null;
		private readonly string ProcessName;

		
		/// <summary>
		/// Scanner object to access all Scanner Function
		/// </summary>
		public Scanner Scanner
		{
			get
			{
				if (_scanner == null)
				{
					throw new NullReferenceException("C0D3RMemory not initialized! Please call constructor with processname first!");
				}
				return _scanner;
			}
			private set
			{
				_scanner = value;
			}
		}
		
		/// <summary>
		/// Reader object to access all ReadMemory Functions
		/// </summary>
		public Reader Reader
		{
			get
			{
				if (_reader == null)
				{
					throw new NullReferenceException("C0D3RMemory not initialized! Please call constructor with processname first!");
				}
				return _reader;
			}
			private set
			{
				_reader = value;
			}
		}
		
		/// <summary>
		/// Writer object to access all WriteMemory Functions
		/// </summary>
		public Writer Writer
		{
			get
			{
				if (_writer == null)
				{
					throw new NullReferenceException("C0D3RMemory not initialized! Please call constructor with processname first!");
				}
				return _writer;
			}
			private set
			{
				_writer = value;
			}
		}
		
		/// <summary>
		/// Current Process's BaseAddress
		/// </summary>
		public IntPtr BaseAddress
		{
			get
			{
				if (curProc != null && curProc.MainModule != null)
				{
					return curProc.MainModule.BaseAddress;
				}
				return IntPtr.Zero;
			}
			private set { }
		}
		
		/// <summary>
		/// Current Process
		/// </summary>
		public Process CurrentProcess
		{
			get
			{
				if (curProc == null)
				{
					throw new NullReferenceException("C0D3RMemory not initialized! Please call constructor with processname first!");
				}
				return curProc;
			}
			private set
			{
				curProc = value;
			}
		}
		
		/// <summary>
		/// Dumps Process's MainModule memory
		/// </summary>
		/// <param name="paddingStart">defines a padding added to the baseaddress</param>
		/// <param name="paddingEnd">defines a padding subtracted from the ModuleSize</param>
		/// <returns>a byte array containing all module data if succeed, else an empty byte array</returns>
		public byte[] DumpMemory(ulong paddingStart = 0, ulong paddingEnd = 0)
		{
			if (curProc == null) return Array.Empty<byte>();
			if (curProc.MainModule == null) return Array.Empty<byte>();
			if (paddingEnd < 0) paddingEnd = 0;

			ulong size = (ulong)curProc.MainModule.ModuleMemorySize - paddingEnd;
			if (paddingStart > size) return Array.Empty<byte>();

			byte[] buffer = new byte[size];
			API.ReadProcessMemory(ProcessHandle, BaseAddress + (nint)paddingStart, buffer, size, out nint bytesRead);
			return buffer;
		}
		~C0D3RMemory() { if (ProcessHandle != IntPtr.Zero) API.CloseHandle(ProcessHandle); }

		/// <summary>
		/// Initialize MemoryManager by the given Processname
		/// </summary>
		/// <param name="procName">Processname it should open a handle to</param>
		public C0D3RMemory(string procName)
		{
			this.ProcessName = procName;
			if (! this.OpenProcessHandle())
			{
				throw new Exception("C0D3RMemory could not be initialized! Open Process Handle failed!");
			}
		}

		private bool OpenProcessHandle()
		{
			if (ProcessHandle != IntPtr.Zero) API.CloseHandle(ProcessHandle);
			bool success = false;

			Process[] Processes = Process.GetProcessesByName(ProcessName);
			if (Processes.Length > 0)
			{
				curProc = Processes[0];
				if (curProc.MainModule != null)
				{
					ProcessHandle = API.OpenProcess(
						API.PROCESS_VM_READ
						| API.PROCESS_VM_WRITE
						| API.PROCESS_VM_OPERATION,
						false,
						curProc.Id
						);
					if (ProcessHandle != IntPtr.Zero)
					{
						this.Reader = new(ProcessHandle);
						this.Writer = new(this.ProcessHandle, this.Reader);
					}
					success = ProcessHandle != IntPtr.Zero;
				}
			}
			return success;
		}

	}
}