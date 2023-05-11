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

namespace Atelier.Controllers
{
    public class VetementsController : Controller
    {
        private readonly ApplicationDbContext context;
        private IAuthorizationService AuthorizationService { get; }
        private UserManager<IdentityUser> UserManager { get; }

        public VetementsController(ApplicationDbContext context, IAuthorizationService authorizationService, UserManager<IdentityUser> userManager) 
        {
            this.context = context;
            AuthorizationService = authorizationService;
            UserManager = userManager; 
        }

        // GET: Vetements
        public async Task<IActionResult> Index() 
        { 
            if (context.Vetement == null) 
                return NotFound(); 

            var vetements = from v in context.Vetement select v; 
            var isAuthorized = User.IsInRole(AuthorizationConstants.VetementAdministratorsRole); 
            var currentUserId = UserManager.GetUserId(User); 
            if (!isAuthorized)
                vetements = vetements.Where(v => v.ProprietaireId == currentUserId);
            return View(await vetements.ToListAsync()); }

        // GET: Aleatoire
        public async Task<IActionResult> Random()
        {
            if (context.Vetement == null)
                return NotFound();

            var isAuthorized = User.IsInRole(AuthorizationConstants.VetementAdministratorsRole);

            //CREATION TENU RANDOM

            //RECUPERATION DE DONNEES
            /*var vetements = from v in context.Vetement select v;
            var currentUserId = UserManager.GetUserId(User);
            var rnd = new Random();

            //RANDOMISATEUR
            var RandomVetements = from v in context.Vetement select v;
            var randomNumber = 1;

            //DONNES
            List<int> idRandomTenu = new List<int>();
            var vetementsChoisis = from v in context.Vetement select v;
            int query;
            var RandomTenu = from v in context.Vetement select v;*/
            List<int> idRandomTenu = new List<int>();

            var rnd = new Random();
            var RandomTenu = from v in context.Vetement select v;
            var vetements = from v in context.Vetement select v;

            //TENU
            bool IsBody = false;

            if (!isAuthorized)
            {
                //HAUT
                try
                {
                    idRandomTenu.Add(ChoixPieceRandom("haut"));
                }
                catch (Exception e)
                {
                }

                //VERIFICATION DE TENU BODY
                try
                {
                    if (vetements.Where(t => idRandomTenu.Contains(t.VetementId)).First().IsTenu2 == "oui")
                        IsBody = true;
                }
                catch (Exception e)
                {
                }

                //TETE
                try
                {
                    if (rnd.Next(0, 2) == 1)
                        idRandomTenu.Add(ChoixPieceRandom("tete"));
                }
                catch (Exception e)
                {
                }

                //HAUT
                try
                {
                    if (!IsBody)
                        idRandomTenu.Add(ChoixPieceRandom("bas"));
                }
                catch (Exception e)
                {
                }

                //SOUS VETEMENT
                try
                {
                    idRandomTenu.Add(ChoixPieceRandom("dessous"));
                }
                catch (Exception e)
                {
                }

                //PIED
                try
                {
                    idRandomTenu.Add(ChoixPieceRandom("pied"));
                }
                catch (Exception e)
                {
                }

                //SELECT LA TENU
                RandomTenu = vetements.Where(t => idRandomTenu.Contains(t.VetementId));
            }
            return View(await RandomTenu.ToListAsync());
        }

        private int ChoixPieceRandom(string place)
        {
            //RECUPERATION DE DONNEES
            var vetements = from v in context.Vetement select v;
            var currentUserId = UserManager.GetUserId(User);

            //SELECT LES VETEMENTS DU TYPE PLACE
            var vetementsChoisis = vetements.Where(v => v.ProprietaireId == currentUserId && v.Type == place);

            //CHOISIS UN VETEMENT RANDOM
            var rnd = new Random();
            var randomNumber = rnd.Next(0, vetementsChoisis.Count());
            var RandomVetements = vetementsChoisis.Skip(randomNumber).Take(1);

            //RETURN
            return RandomVetements.First().VetementId;
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
        public async Task<IActionResult> Create([Bind("VetementId,Nom,Description,DateObtention,Type,IsTenu2,Image")] Vetement vetement)
        {
            if (ModelState.IsValid)
            {
                vetement.ProprietaireId = UserManager.GetUserId(User);
                var isAuthorized = await AuthorizationService.AuthorizeAsync(
                    User, vetement, VetementOperations.Create);

                if (!isAuthorized.Succeeded)
                    return Forbid();

                context.Add(vetement);
                await context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(vetement);
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
        public async Task<IActionResult> Edit(int id, [Bind("VetementId,Nom,Description,DateObtention,Type,IsTenu2,Image")] Vetement vetement)
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
