﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ScottPlot.Cookbook
{
    public static class RecipeImages
    {
        /// <summary>
        /// Use REFLECTION to locate all recipes, execute them and save the output images.
        /// </summary>
        /// <returns>array of recipes found using reflection</returns>
        public static IRecipe[] Generate(string outputPath, int width = 600, int height = 400, int thumbJpegQuality = 95)
        {
            outputPath = Path.GetFullPath(outputPath);
            if (!Directory.Exists(outputPath))
                Directory.CreateDirectory(outputPath);

            IRecipe[] recipes = Locate.GetRecipes();

            EncoderParameters thumbJpegEncoderParameters = new(1);
            thumbJpegEncoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, thumbJpegQuality);
            ImageCodecInfo thumbJpegEncoder = ImageCodecInfo.GetImageEncoders().Where(x => x.MimeType == "image/jpeg").First();

            Parallel.ForEach(recipes, recipe =>
            {
                var plt = new Plot(width, height);
                recipe.ExecuteRecipe(plt);

                // save full size image
                Bitmap bmp = plt.Render();
                string filePath = Path.Combine(outputPath, recipe.ID.ToLower() + ".png");
                bmp.Save(filePath, ImageFormat.Png);

                // thumbnail
                int thumbHeight = 180;
                int thumbWidth = thumbHeight * bmp.Width / bmp.Height;
                Bitmap thumb = Drawing.GDI.Resize(bmp, thumbWidth, thumbHeight);
                string thumbFilePath = Path.Combine(outputPath, recipe.ID.ToLower() + "_thumb.jpg");
                thumb.Save(thumbFilePath, thumbJpegEncoder, thumbJpegEncoderParameters);
            });

            return recipes;
        }
    }
}
