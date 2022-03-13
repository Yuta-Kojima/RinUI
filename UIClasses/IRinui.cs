namespace RinUI
{
    public interface IRinui
    {
        public Vector2x2 Vector { get; }
        public byte Layer { get; }
        public Style Style { get; }


        public void Update();
        public void Draw();
    }
}
