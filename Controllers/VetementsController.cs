using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Atelier.Data;
using Atelier.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Atelier.Authorizations;
using Atelier.Models;

namespace Atelier.Controllers
{
    public class VetementsController : Controller
    {
        private readonly ApplicationDbContext context;
        private IAuthorizationService AuthorizationService { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private readonly IWebHostEnvironment webHostEnvironment;

        public VetementsController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager, IWebHostEnvironment webHostEnvironment)
        {
            this.context = context;
            AuthorizationService = authorizationService;
            UserManager = userManager;
            this.webHostEnvironment = webHostEnvironment;
        }

        // GET: Vetements
        public async Task<IActionResult> Index(string vetementType, string searchString)
        {
            if (context.Vetement == null)
                return NotFound();

            IQueryable<string> typeQuery = from m in context.Vetement
                                           orderby m.Type
                                           select m.Type;

            var vetements = from m in context.Vetement
                            select m;
            var isAuthorized = User.IsInRole(AuthorizationConstants.VetementAdministratorsRole);
            var currentUserId = UserManager.GetUserId(User);
            var vetementTypeVM = new VetementTypeViewModel();
            if (!isAuthorized)
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    vetements = vetements.Where(s => s.Nom!.Contains(searchString));
                }

                if (!string.IsNullOrEmpty(vetementType))
                {
                    vetements = vetements.Where(x => x.Type == vetementType);
                }

                var vetementTempVM = new VetementTypeViewModel
                {
                    Type = new SelectList(await typeQuery.Distinct().ToListAsync()),
                    Vetements = await vetements.Where(v => v.ProprietaireId == currentUserId).ToListAsync()
                };
                vetementTypeVM = vetementTempVM;
            }
            return View(vetementTypeVM);
        }

        // GET: Vetements/Details/5
        public async Task<IActionResult> Details(int? id)
        { 
            if (id == null || context.Vetement == null)
            { 
                return NotFound(); 
            } 
            var v = await context.Vetement.FirstOrDefaultAsync(m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(User, v, VetementOperations.Read);
            if (!isAuthorized.Succeeded) 
                return Forbid();
            return View(v);
        }

        // GET: Vetements/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Vetements/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VetementId,Nom,Description,DateObtention,Type,ImageVetement")] VetementAddViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                model.ProprietaireId = UserManager.GetUserId(User);
                

                string uniqueFileName = UploadedFile(model);
                Vetement vetement = new Vetement
                {
                    ProprietaireId = model.ProprietaireId,
                    VetementId = model.VetementId,
                    Nom = model.Nom,
                    Description = model.Description,
                    DateObtention = model.DateObtention,
                    Type = model.Type,
                    ImageVetement = uniqueFileName,
                };

                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                    User, vetement, VetementOperations.Create);

                if (!isAuthorized.Succeeded)
                    return Forbid();

                context.Add(vetement);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private string UploadedFile(VetementAddViewModel model)
        {
            string uniqueFileName = null;

            if (model.ImageVetement != null)
            {
                string uploadsFolder = Path.Combine(webHostEnvironment.WebRootPath, "images");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ImageVetement.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.ImageVetement.CopyTo(fileStream);
                }
            }
            return uniqueFileName;
        }

        // GET: Vetements/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || context.Vetement == null)
            {
                return NotFound();
            }

            var vetement = await context.Vetement.FindAsync(id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                   User, vetement, VetementOperations.Update);

            if (!isAuthorized.Succeeded)
                return Forbid();

            if (vetement == null)
                return NotFound();
            return View(vetement);
        }

        // POST: Vetements/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VetementId,Nom,Description,DateObtention,Type,Image")] Vetement vetement)
        {
           

            if (id != vetement.VetementId)
            {
                return NotFound();
            }

            var v = await context.Vetement.AsNoTracking().FirstOrDefaultAsync(m => m.VetementId == id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Update);

            vetement.ProprietaireId = v.ProprietaireId;

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    context.Update(vetement);
                    await context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VetementExists(vetement.VetementId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(vetement);
        }

        // GET: Vetements/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || context.Vetement == null)
            {
                return NotFound();
            }

            var v = await context.Vetement
                .FirstOrDefaultAsync(m => m.VetementId == id);

            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Delete);

            if (!isAuthorized.Succeeded)
                return Forbid();
            if (v == null)
                return NotFound();

            return View(v);
        }

        // POST: Vetements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (context.Vetement == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Vetement'  is null.");
            }
            var v = await context.Vetement.FindAsync(id);
            var isAuthorized = await AuthorizationService.AuthorizeAsync(
                User, v, VetementOperations.Delete);

            if (!isAuthorized.Succeeded)
                return Forbid();

            if (v == null)
                return NotFound();
                
            context.Vetement.Remove(v);
            await context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VetementExists(int id)
        {
          return (context.Vetement?.Any(e => e.VetementId == id)).GetValueOrDefault();
        }
    }
}
