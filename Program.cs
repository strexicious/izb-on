using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Assimp;
using OpenTK.Graphics.OpenGL;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using IZBPipeline;

namespace apur_on
{
	struct EngineSettings
	{
		public GameWindowSettings GameWindowSettings;
		public NativeWindowSettings NativeWindowSettings;
		public Vector2i WindowSize;
	}

	class Engine : GameWindow
	{
		private List<Mesh> Scene = new List<Mesh>();
		private IZBRenderer IZBRenderer;
		private Camera DefaultCamera;
		private AssimpContext AssimpContext = new AssimpContext();

		private float CamSpeed = 0.1f;
		
		static void DebugCallback(DebugSource source, DebugType type, int id, DebugSeverity severity, int length, IntPtr message, IntPtr userParam)
		{
			string sev;
			switch (severity) {
				case DebugSeverity.DebugSeverityHigh: sev = "\u001b[91m"; break;
				case DebugSeverity.DebugSeverityMedium: sev = "\u001b[93m"; break;
				case DebugSeverity.DebugSeverityLow: sev = "\u001b[92m"; break;
				case DebugSeverity.DebugSeverityNotification: sev = "\u001b[34m"; break;
				default: sev = "\u001b[92m"; break;
			}

			string src;
			switch (source) {
				case DebugSource.DebugSourceApi: src = "API"; break;
				case DebugSource.DebugSourceWindowSystem: src = "window system"; break;
				case DebugSource.DebugSourceShaderCompiler: src = "shader compiler"; break;
				case DebugSource.DebugSourceThirdParty: src = "third party"; break;
				case DebugSource.DebugSourceApplication: src = "app"; break;
				case DebugSource.DebugSourceOther:
				default: src = "other"; break;
			}

			string type_str;
			switch (type) {
				case DebugType.DebugTypeError: type_str = "error"; break;
				case DebugType.DebugTypeDeprecatedBehavior: type_str = "deprecated behavior"; break;
				case DebugType.DebugTypeUndefinedBehavior: type_str = "undefined behavior"; break;
				case DebugType.DebugTypePortability: type_str = "portability"; break;
				case DebugType.DebugTypeMarker: type_str = "marker"; break;
				case DebugType.DebugTypePushGroup: type_str = "push group"; break;
				case DebugType.DebugTypePopGroup: type_str = "pop group"; break;
				case DebugType.DebugTypePerformance: type_str = "performance"; break;
				case DebugType.DebugTypeOther:
				default: type_str = "other"; break;
			}

			string messageStr = Marshal.PtrToStringAnsi(message);

			System.Console.Error.WriteLine($"debug:{sev} type: {type_str}, source: {src}, message: \"{messageStr}\"\u001b[0m");
		}
		
		public Engine(EngineSettings settings) : base(settings.GameWindowSettings, settings.NativeWindowSettings) {
			GL.Enable(EnableCap.DebugOutput);
			GL.Enable(EnableCap.DebugOutputSynchronous);
			GL.DebugMessageCallback(DebugCallback, IntPtr.Zero);

			LoadScene("wide_monkey.obj");
			DefaultCamera = new Camera(settings.WindowSize.X, settings.WindowSize.Y, 0.01f, 100.0f);

			IZBRenderer = new IZBRenderer(Scene, DefaultCamera);
		}

		public void LoadScene(string name)
		{
			Scene scene = AssimpContext.ImportFile("./res/models/" + name, PostProcessSteps.Triangulate);
			scene.Meshes.ForEach(m => Scene.Add(new Mesh(m)));
		}

		protected override void OnLoad()
		{
			GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);
			GL.Enable(EnableCap.DepthTest);
			
			base.OnLoad();
		}

		protected override void OnUpdateFrame(FrameEventArgs args)
		{
			if (IsKeyDown(Keys.W))
			{
				DefaultCamera.Position += Vector3.Multiply(DefaultCamera.Direction, CamSpeed);
				IZBRenderer.UpdateCam();
			}
			
			if (IsKeyDown(Keys.S))
			{
				DefaultCamera.Position -= Vector3.Multiply(DefaultCamera.Direction, CamSpeed);
				IZBRenderer.UpdateCam();
			}
			
			base.OnUpdateFrame(args);
		}

		protected override void OnMouseMove(MouseMoveEventArgs e)
		{
			DefaultCamera.MoveDirection(e.DeltaX, e.DeltaY);
				IZBRenderer.UpdateCam();
			
			base.OnMouseMove(e);
		}

		protected override void OnRenderFrame(FrameEventArgs args)
		{
			GL.Clear(ClearBufferMask.ColorBufferBit);

			IZBRenderer.Render();

			SwapBuffers();
			base.OnRenderFrame(args);
		}

		protected override void Dispose(bool disposing)
		{
			// Dispose(bool disposing) executes in two distinct scenarios.
			// If disposing equals true, the method has been called directly
			// or indirectly by a user's code. Managed and unmanaged resources
			// can be disposed.
			// If disposing equals false, the method has been called by the
			// runtime from inside the finalizer and you should not reference
			// other objects. Only unmanaged resources can be disposed.
			//
			// from: https://docs.microsoft.com/en-us/dotnet/api/system.idisposable.dispose?view=netcore-3.1

			// finalizer is the destructor, now even though OpenGL's resources
			// are unmanaged, we can safely only access our fields if disposing = true
			if (disposing)
			{
				Scene.ForEach(m => m.Unassign());
				IZBRenderer.Unassign();
				AssimpContext.Dispose();
			}

			base.Dispose(disposing);
		}
	}

	class Program
	{
		static void Main(string[] args)
		{
			EngineSettings engineSettings;
			engineSettings.GameWindowSettings = GameWindowSettings.Default;
			engineSettings.NativeWindowSettings = NativeWindowSettings.Default;
			engineSettings.NativeWindowSettings.Flags |= ContextFlags.Debug;
			engineSettings.NativeWindowSettings.Size =
				engineSettings.WindowSize = new Vector2i(1600, 800);

			using (Engine engine = new Engine(engineSettings))
			{
				engine.Run();
			}
		}
	}
}
