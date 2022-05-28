
namespace MailSpamer
{
   public class CustomContainer
    {
        public string _text { get; set; }
        public int _index { get; set; }

        public CustomContainer(string name, int _num)
        {
            _text = name;
            _index = _num;
        }
    }
}
