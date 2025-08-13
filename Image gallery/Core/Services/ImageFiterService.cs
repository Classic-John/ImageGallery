using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Interfaces;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;


namespace Core.Services
{
    public class ImageFiterService : IImageFilterService
    {
        public byte[] ApplyChosenFilter(byte[]? image, int? filter)
        {
            if (image is null)
                return null;
            Image trueImage = Image.Load(new MemoryStream(image));
            switch (filter.Value)
            {
                case 1:
                    trueImage.Mutate(filterType => filterType.BlackWhite());
                    break;
                case 2:
                    trueImage.Mutate(filterType => filterType.Sepia());
                    break;
                case 3:
                    trueImage.Mutate(filterType => filterType.Grayscale());
                    break;
                case 4:
                    trueImage.Mutate(filterType => filterType.Vignette());
                    break;
                case 5:
                    trueImage.Mutate(filterType => filterType.Invert());
                    break;
                case 6:
                    trueImage.Mutate(filterType => filterType.GaussianBlur());
                    break;
                case 7:
                    trueImage.Mutate(filterType => filterType.Rotate(90));
                    break;
                case 8:
                    trueImage.Mutate(filterType => filterType.Rotate(-90));
                    break;
                case 9:
                    trueImage.Mutate(filterType => filterType.RotateFlip(RotateMode.None,FlipMode.Vertical));
                    break;
                case 10:
                    trueImage.Mutate(filterType => filterType.RotateFlip(RotateMode.None, FlipMode.Horizontal));
                    break;
            }
            MemoryStream output = new();
            trueImage.SaveAsJpeg(output);
            return output.ToArray();
        }
    }
}
