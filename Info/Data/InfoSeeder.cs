using Info.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Info.Data
{
    public class InfoSeeder
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var dbContext = services.GetRequiredService<ApplicationDbContext>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<AppUser>>();

                if (!await dbContext.Database.CanConnectAsync())
                {
                    throw new Exception(
                        "Nie można połączyć się z bazą danych.");
                }

                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
                await SeedCategoriesAsync(dbContext);
                await SeedTextsAsync(dbContext);
                await SeedOpinionsAsync(dbContext, userManager);
            }
            catch (Exception ex)
            {
                throw new Exception("Błąd podczas inicjalizacji danych.", ex);
            }
        }

        // Dodawanie ról
        private static async Task SeedRolesAsync(
            RoleManager<IdentityRole> roleManager)
        {
            string[] roleNames = { "admin", "author" };

            foreach (var roleName in roleNames)
            {
                if (await roleManager.RoleExistsAsync(roleName))
                    continue;

                var result =
                    await roleManager.CreateAsync(
                        new IdentityRole(roleName));

                if (!result.Succeeded)
                {
                    var errors = string.Join(
                        ", ",
                        result.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Nie udało się utworzyć roli {roleName}. " +
                        $"Błędy: {errors}");
                }
            }
        }

        // Dodawanie użytkowników
        private static async Task SeedUsersAsync(
            UserManager<AppUser> userManager)
        {
            const string seedPassword = "Portalik1!";

            var usersToCreate =
                new List<(AppUser User, string Role)>
            {
                (new AppUser
                {
                    UserName = "autor1@portal.pl",
                    Email = "autor1@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Piotr",
                    LastName = "Pisarski",
                    Photo = "autor1.jpg",
                    Information =
                        "Wszechstronny programista aplikacji " +
                        "sieciowych i internetowych. W portfolio " +
                        "ma kilka ciekawych projektów zrealizowanych " +
                        "dla firm z branży finansowej. Współpracuje " +
                        "z innowacyjnymi startupami."
                }, "author"),

                (new AppUser
                {
                    UserName = "autor2@portal.pl",
                    Email = "autor2@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Anna",
                    LastName = "Autorska",
                    Photo = "autor2.jpg",
                    Information =
                        "Doświadczona programistka i projektantka " +
                        "stron internetowych oraz uznana blogierka. " +
                        "Specjalizuje się w HTML5, CSS3, JavaScript, " +
                        "jQuery i Bootstrap. Obecnie pracuje nad " +
                        "nowymi rozwiązaniami dla graczy."
                }, "author"),

                (new AppUser
                {
                    UserName = "admin@portal.pl",
                    Email = "admin@portal.pl",
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    FirstName = "Ewa",
                    LastName = "Ważna",
                    Photo = "woman.png",
                    Information = string.Empty
                }, "admin")
            };

            foreach (var (newUser, role) in usersToCreate)
            {
                var user =
                    await userManager.FindByEmailAsync(
                        newUser.Email!);

                if (user == null)
                {
                    var createResult =
                        await userManager.CreateAsync(
                            newUser,
                            seedPassword);

                    if (!createResult.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            createResult.Errors.Select(
                                e => e.Description));

                        throw new Exception(
                            $"Błąd podczas tworzenia użytkownika " +
                            $"{newUser.UserName}: {errors}");
                    }

                    user = newUser;
                }

                if (!await userManager.IsInRoleAsync(user, role))
                {
                    var roleResult =
                        await userManager.AddToRoleAsync(
                            user,
                            role);

                    if (!roleResult.Succeeded)
                    {
                        var errors = string.Join(
                            ", ",
                            roleResult.Errors.Select(
                                e => e.Description));

                        throw new Exception(
                            $"Błąd podczas przypisywania roli " +
                            $"dla {user.UserName}: {errors}");
                    }
                }
            }
        }

        // Dodawanie kategorii
        private static async Task SeedCategoriesAsync(
            ApplicationDbContext dbContext)
        {
            var categoriesToCreate = new List<Category>
            {
                new()
                {
                    Name = "Wiadomości",
                    Active = true,
                    Display = true,
                    Icon = "chat-left-text",
                    Description =
                        "Najświeższe wiadomości i informacje z dziedziny informatyki. " +
                        "Coś dla programistów i zwykłych użytkowników komputerów, " +
                        "tabletów oraz smartfonów."
                },

                new()
                {
                    Name = "Artykuły",
                    Active = true,
                    Display = true,
                    Icon = "journal-richtext",
                    Description =
                        "Artykuły w naszym serwisie pisane są przez wybitnych znawców " +
                        "tematu, którzy z olbrzymią przenikliwością zgłębiają każdy temat."
                },

                new()
                {
                    Name = "Testy",
                    Active = true,
                    Display = true,
                    Icon = "speedometer",
                    Description =
                        "Nasze laboratorium testuje dla Was najnowszy sprzęt, poddając go " +
                        "elektronicznym torturom i wyciskając siódme poty elektronów."
                },

                new()
                {
                    Name = "Porady",
                    Active = true,
                    Display = true,
                    Icon = "life-preserver",
                    Description =
                        "Jeżeli wciąż masz problemy z obsługą komputerów lub chcesz " +
                        "pracować efektywnie zajrzyj do sekcji z poradami dla adminów i laików."
                },

                new()
                {
                    Name = "Tutoriale",
                    Active = true,
                    Display = true,
                    Icon = "display",
                    Description =
                        "W tutorialach opisujemy krok po kroku, w jaki sposób rozwiązać " +
                        "zadania programistyczne praktycznie z każdej dziedziny."
                },

                new()
                {
                    Name = "Recenzje",
                    Active = true,
                    Display = true,
                    Icon = "controller",
                    Description =
                        "Czytamy najciekawsze książki informatyczne i gramy dla Was w " +
                        "najnowsze gry, aby później opisać je dokładnie w tym dziale."
                }
            };

            foreach (var category in categoriesToCreate)
            {
                if (!await dbContext.Categories
                    .AnyAsync(c => c.Name == category.Name))
                {
                    await dbContext.Categories.AddAsync(category);
                }
            }

            await dbContext.SaveChangesAsync();
        }

        // Dodawanie tekstów
        private static async Task SeedTextsAsync(ApplicationDbContext dbContext)
        {
            if (await dbContext.Texts.AnyAsync())
                return;

            var autor1Id = await dbContext.Users
                .Where(u => u.UserName == "autor1@portal.pl")
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            var autor2Id = await dbContext.Users
                .Where(u => u.UserName == "autor2@portal.pl")
                .Select(u => u.Id)
                .FirstOrDefaultAsync();

            if (autor1Id == null || autor2Id == null)
                throw new Exception(
                    "Nie znaleziono wymaganych autorów w bazie danych.");

            string[] categoryNames =
            {
                "Wiadomości",
                "Artykuły",
                "Testy",
                "Porady",
                "Tutoriale",
                "Recenzje"
            };

            var categories = await dbContext.Categories
                .Where(c => categoryNames.Contains(c.Name))
                .ToListAsync();

            if (categories.Count != categoryNames.Length)
                throw new Exception(
                    "Nie znaleziono wszystkich wymaganych kategorii w bazie danych.");

            // Zachowujemy zadaną kolejność kategorii niezależnie od ich CategoryId.
            var orderedCategories = categoryNames
                .Select(name => categories.Single(c => c.Name == name))
                .ToList();

            var texts = new List<Text>();
            int textNumber = 1;

            foreach (var category in orderedCategories)
            {
                // 5 tekstów autora 1 i 5 tekstów autora 2 w każdej kategorii.
                for (int j = 1; j <= 10; j++)
                {
                    string authorId =
                        j <= 5 ? autor1Id : autor2Id;

                    texts.Add(new Text
                    {
                        Title = $"Tytuł {textNumber}",

                        Summary = $"Streszczenie tekstu o tytule Tytuł {textNumber}",

                        Keywords = $"tag{textNumber}, tag{category.Name.ToLowerInvariant()}",

                        Content =
                            "Lorem ipsum dolor sit amet, consectetur adipiscing elit, sed do " +
                            "eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim " +
                            "ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut " +
                            "aliquip ex ea commodo consequat. Duis aute irure dolor in " +
                            "reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla " +
                            "pariatur. Excepteur sint occaecat cupidatat non proident, sunt in " +
                            "culpa qui officia deserunt mollit anim id est laborum. Sed ut " +
                            "perspiciatis unde omnis iste natus error sit voluptatem accusantium " +
                            "doloremque laudantium, totam rem aperiam, eaque ipsa quae ab illo " +
                            "inventore veritatis et quasi architecto beatae vitae dicta sunt " +
                            "explicabo. Nemo enim ipsam voluptatem quia voluptas sit aspernatur " +
                            "aut odit aut fugit, sed quia consequuntur magni dolores eos qui " +
                            "ratione voluptatem sequi nesciunt. Neque porro quisquam est, qui " +
                            "dolorem ipsum quia dolor sit amet, consectetur, adipisci velit, sed " +
                            "quia non numquam eius modi tempora incidunt ut labore et dolore " +
                            "magnam aliquam quaerat voluptatem. Ut enim ad minima veniam, quis " +
                            "nostrum exercitationem ullam corporis suscipit laboriosam, nisi ut " +
                            "aliquid ex ea commodi consequatur? Quis autem vel eum iure " +
                            "reprehenderit qui in ea voluptate velit esse quam nihil molestiae " +
                            "consequatur, vel illum qui dolorem eum fugiat quo voluptas nulla " +
                            "pariatur?",

                        AddedDate = DateTime.Now.AddDays(-textNumber),
                        Category = category,
                        UserId = authorId,
                        Active = true
                    });

                    textNumber++;
                }
            }

            await dbContext.Texts.AddRangeAsync(texts);
            await dbContext.SaveChangesAsync();
        }

        // Dodawanie opinii
        private static async Task SeedOpinionsAsync(
            ApplicationDbContext dbContext,
            UserManager<AppUser> userManager)
        {
            // Jeżeli w bazie są już opinie, nie generujemy kolejnych.
            if (await dbContext.Opinions.AnyAsync())
                return;

            var admin = await userManager.FindByEmailAsync("admin@portal.pl");

            var autor1 = await userManager.FindByEmailAsync("autor1@portal.pl");

            var autor2 = await userManager.FindByEmailAsync("autor2@portal.pl");

            if (admin == null || autor1 == null || autor2 == null)
            {
                throw new Exception(
                    "Nie znaleziono użytkowników wymaganych do utworzenia opinii.");
            }

            // Pobieramy rzeczywiste identyfikatory tekstów
            // oraz identyfikatory ich autorów.
            var texts = await dbContext.Texts
                .OrderBy(t => t.TextId)
                .Select(t => new
                {
                    t.TextId,
                    t.UserId
                })
                .ToListAsync();

            if (texts.Count == 0)
            {
                throw new Exception(
                    "Brak tekstów, do których można dodać opinie.");
            }

            var opinions = new List<Opinion>();
            int opinionNumber = 1;

            foreach (var text in texts)
            {
                // Pierwszą opinię do każdego tekstu dodaje administrator.
                opinions.Add(new Opinion
                {
                    Comment =
                        "Sed ut perspiciatis unde omnis iste natus error sit voluptatem " +
                        "accusantium doloremque laudantium, totam rem aperiam, eaque ipsa " +
                        "quae ab illo inventore veritatis et quasi architecto beatae vitae " +
                        "dicta sunt explicabo.",

                    AddedDate = DateTime.Now.AddDays(-opinionNumber),
                    UserId = admin.Id,
                    TextId = text.TextId,
                    Rating = Rating.Excellent
                });

                opinionNumber++;

                // Drugą opinię dodaje autor, który NIE napisał danego tekstu.
                AppUser reviewingAuthor;

                if (text.UserId == autor1.Id)
                {
                    reviewingAuthor = autor2;
                }
                else if (text.UserId == autor2.Id)
                {
                    reviewingAuthor = autor1;
                }
                else
                {
                    throw new Exception(
                        $"Tekst o identyfikatorze {text.TextId} nie ma oczekiwanego autora.");
                }

                opinions.Add(new Opinion
                {
                    Comment =
                        "At vero eos et accusamus et iusto odio dignissimos ducimus qui " +
                        "blanditiis praesentium voluptatum deleniti atque corrupti quos " +
                        "dolores et quas molestias excepturi sint occaecati cupiditate " +
                        "non provident.",

                    AddedDate = DateTime.Now.AddDays(-opinionNumber),
                    UserId = reviewingAuthor.Id,
                    TextId = text.TextId,
                    Rating = Rating.Good
                });

                opinionNumber++;
            }

            await dbContext.Opinions.AddRangeAsync(opinions);
            await dbContext.SaveChangesAsync();
        }
    }
}
