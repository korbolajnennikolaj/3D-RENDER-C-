using System.Drawing;
using System.Numerics;
using System.Reflection;
using Geometry;
using SFML.Graphics;
using SFML.System;
using Shading;

namespace d2D{    
    public class d2DScreen{
        private uint _sizeX = 0;
        private uint _sizeY = 0;

        public uint SizeX { get{
            if (window != null) return window.Size.X;
            return _sizeX;
        } 
        set{
            if (window != null){
                window.Size = new SFML.System.Vector2u(value, SizeY);
            }
            else
            {
                _sizeX = value;
            }
        }}

        public uint SizeY { get{
            if (window != null) return window.Size.Y;
            return _sizeY;
        } 
        set{
            if (window != null){
                window.Size = new SFML.System.Vector2u(SizeX, value);
            }
            else
            {
                _sizeY = value;
            }
        }}

        private string _titleWindow = "";
        public string TitleWindow {get => _titleWindow; set{
            _titleWindow = value;
            window?.SetTitle(value);
        }}

        public bool HasFocus{get{
            if (window != null) return window.HasFocus();
            else return false;
        }}

        private SFML.Graphics.RenderWindow? window;
        
        public delegate void InitEventHandler();
        public event InitEventHandler? OnInit;

        public delegate void CloseEventHandler();
        public event CloseEventHandler? OnClose;

        public delegate void ResizeEventHandler(float Width, float Height);
        public event ResizeEventHandler? OnResize;

        public delegate void RenderEventHandler(float DeltaTime);
        public event RenderEventHandler? OnRender;

        public d2DScreen(uint x, uint y){
            SizeX = x;
            SizeY = y;
        }

        public void Run(){
            SFML.Window.VideoMode mode = new SFML.Window.VideoMode(SizeX, SizeY);
            window = new SFML.Graphics.RenderWindow(mode, _titleWindow);
            SFML.System.Clock clock = new SFML.System.Clock();

            float LastTime = clock.ElapsedTime.AsSeconds();
            float CurrentTime = 0.0f;
            float DeltaTime = 0.0f;

            window.Closed += (_, _) => OnClose?.Invoke();
            window.Resized += (_, _) => {
                OnResize?.Invoke(SizeX, SizeY);
                window.SetView(new View(new FloatRect(new Vector2f(0, 0), new Vector2f(SizeX, SizeY))));
            };

            OnInit?.Invoke();
            
            while (window.IsOpen){
                window.DispatchEvents();
                window.Clear();

                LastTime = CurrentTime;
                CurrentTime = clock.ElapsedTime.AsSeconds();
                DeltaTime = CurrentTime - LastTime;

                OnRender?.Invoke(DeltaTime);
                window.Display();
            }
        }

        public void Close(){
            window?.Close();
        }

        public void Draw(Drawable Figure){
            window?.Draw(Figure);
        }

        public void SetFramerateLimit(uint Framerate){
            window?.SetFramerateLimit(Framerate);
        }

        public static SizeF GetWindowSize(){
            return new SizeF(SFML.Window.VideoMode.DesktopMode.Width, SFML.Window.VideoMode.DesktopMode.Height); 
        }
    }
}
