using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datalayer.Repositories;

namespace Core
{
    public class UnitOfWork
    {
        public UserRepository? UserRepo { get; set; }
        public TheImageRepository? TheImageRepo { get; set; }
        private readonly ImageGalleryContext _imageGalleryContext;

        public UnitOfWork(UserRepository? userRepo, TheImageRepository? theImageRepo, ImageGalleryContext imageGalleryContext)
        {
            UserRepo = userRepo;
            TheImageRepo = theImageRepo;
            _imageGalleryContext = imageGalleryContext;
            UserRepo.Initialise(imageGalleryContext);
            TheImageRepo.Initialise(imageGalleryContext);
        }
    }
}
