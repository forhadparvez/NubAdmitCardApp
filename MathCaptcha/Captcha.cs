using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace MathCaptcha
{
    public class Captcha
    {
        public static bool CaptchaMatch(string encryptText, string answer)
        {
            var dec = EncryptionHelper.Decrypt(encryptText);
            return dec == answer;
        }

        public static List<string> GetCaptcha()
        {
            var l=new List<string>();
            try
            {
                var number= CaptchaNumber.GetNumber();
                var firstNumber = number[0];
                var secondNumber = number[1];
                var operatorString = CaptchaNumber.GetOperator();

                var answer = CaptchaNumber.GetAnswer(firstNumber, secondNumber, operatorString);

                var image = DrawText(firstNumber+" "+operatorString+" "+secondNumber, new Font("Arial", 20), Color.DarkRed, Color.Bisque);
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, ImageFormat.Bmp);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = "data:image/png;base64," + Convert.ToBase64String(imageBytes);
                    l.Add(base64String);
                    l.Add(EncryptionHelper.Encrypt(answer.ToString()));
                }
            }
            catch (Exception e)
            {
                 throw  new ApplicationException("Captcha Error: "+e.Message);
            }

            return l;
        }

        private static Image DrawText(String text, Font font, Color textColor, Color backColor)
        {
            //first, create a dummy bitmap just to get a graphics object
            Image img = new Bitmap(1, 1);
            Graphics drawing = Graphics.FromImage(img);

            //measure the string to see how big the image needs to be
            SizeF textSize = drawing.MeasureString(text, font);

            //free up the dummy image and old graphics object
            img.Dispose();
            drawing.Dispose();

            //create a new image of the right size
            img = new Bitmap((int)textSize.Width, (int)textSize.Height);

            drawing = Graphics.FromImage(img);

            //paint the background
            drawing.Clear(backColor);

            //create a brush for the text
            Brush textBrush = new SolidBrush(textColor);

            drawing.DrawString(text, font, textBrush, 0, 0);

            drawing.Save();

            textBrush.Dispose();
            drawing.Dispose();

            return img;
        }
    }
}