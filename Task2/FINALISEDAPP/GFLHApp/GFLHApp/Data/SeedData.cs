// ----- Imports -----
using GFLHApp.Models; // Imports GFLHApp.Models types used by this file.
using Microsoft.AspNetCore.Identity; // Imports Microsoft.AspNetCore.Identity types used by this file.
using Microsoft.EntityFrameworkCore; // Imports Microsoft.EntityFrameworkCore types used by this file.

// ----- Namespace -----
namespace GFLHApp.Data // Places this class in the application data namespace.
{
    public class SeedData // Defines database seed helpers for users, producers, and products.
    {

        // ----- Identity configuration -----
        public static async Task SeedUsersAndRoles(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) // Seeds default Identity roles and test users.
        {
            // ----- Seed users and roles -----
            string[] roleNames = { "Admin", "Producer", "Standard", "Developer" }; // Lists the roles required by the application.
            foreach (string roleName in roleNames) // Loops through each item that needs seeding or configuration.
            {
                // ----- Identity configuration -----
                var roleExists = await roleManager.RoleExistsAsync(roleName); // Checks whether the Identity role already exists.
                if (!roleExists) // Checks the condition before continuing this setup step.
                {
                    var role = new IdentityRole(roleName); // Creates the Identity role object for missing roles.
                    await roleManager.CreateAsync(role); // Saves the missing role into Identity.
                }
            }


            var adminUser = await userManager.FindByEmailAsync("admin@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (adminUser == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                adminUser = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(adminUser, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(adminUser, "Admin")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(adminUser, "Admin"); // Assigns the required role to the seeded user.
            }


            var producerUser = await userManager.FindByEmailAsync("producer@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (producerUser == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                producerUser = new IdentityUser { UserName = "producer@example.com", Email = "producer@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(producerUser, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(producerUser, "Producer")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(producerUser, "Producer"); // Assigns the required role to the seeded user.
            }

            var producerUser2 = await userManager.FindByEmailAsync("producer2@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (producerUser2 == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                producerUser2 = new IdentityUser { UserName = "producer2@example.com", Email = "producer2@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(producerUser2, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(producerUser2, "Producer")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(producerUser2, "Producer"); // Assigns the required role to the seeded user.
            }

            var producerUser3 = await userManager.FindByEmailAsync("producer3@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (producerUser3 == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                producerUser3 = new IdentityUser { UserName = "producer3@example.com", Email = "producer3@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(producerUser3, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(producerUser3, "Producer")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(producerUser3, "Producer"); // Assigns the required role to the seeded user.
            }


            var producerUser4 = await userManager.FindByEmailAsync("producer4@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (producerUser4 == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                producerUser4 = new IdentityUser { UserName = "producer4@example.com", Email = "producer4@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(producerUser4, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(producerUser4, "Producer")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(producerUser4, "Producer"); // Assigns the required role to the seeded user.
            }

            var producerUser5 = await userManager.FindByEmailAsync("producer5@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (producerUser5 == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                producerUser5 = new IdentityUser { UserName = "producer5@example.com", Email = "producer5@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(producerUser5, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(producerUser5, "Producer")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(producerUser5, "Producer"); // Assigns the required role to the seeded user.
            }

            var devUser = await userManager.FindByEmailAsync("developer@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (devUser == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                devUser = new IdentityUser { UserName = "developer@example.com", Email = "developer@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(devUser, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(devUser, "Developer")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(devUser, "Developer"); // Assigns the required role to the seeded user.
            }

            var normalUser = await userManager.FindByEmailAsync("user@example.com"); // Looks up the seeded Identity user by email address.
            // ----- Seed users and roles -----
            if (normalUser == null) // Checks the condition before continuing this setup step.
            {
                // ----- Identity configuration -----
                normalUser = new IdentityUser { UserName = "user@example.com", Email = "user@example.com", EmailConfirmed = true }; // Creates the Identity user object for seeding.
                await userManager.CreateAsync(normalUser, "Password123!"); // Creates the seeded user with the default test password.
            }

            if (!await userManager.IsInRoleAsync(normalUser, "Standard")) // Checks whether the seeded user already has the role.
            {
                await userManager.AddToRoleAsync(normalUser, "Standard"); // Assigns the required role to the seeded user.
            }


            {

            }
        }

        // ----- Seed producers -----
        public static async Task SeedProducers(IServiceProvider serviceProvider) // Seeds producer profile records after producer users exist.
        {
            // ----- Identity configuration -----
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); // Resolves a required service from dependency injection.
            // ----- Database configuration -----
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Resolves a required service from dependency injection.

            // ----- Identity configuration -----
            var producerUser1 = await userManager.FindByEmailAsync("producer@example.com"); // Looks up the seeded Identity user by email address.
            var producerUser2 = await userManager.FindByEmailAsync("producer2@example.com"); // Looks up the seeded Identity user by email address.
            var producerUser3 = await userManager.FindByEmailAsync("producer3@example.com"); // Looks up the seeded Identity user by email address.
            var producerUser4 = await userManager.FindByEmailAsync("producer4@example.com"); // Looks up the seeded Identity user by email address.
            var producerUser5 = await userManager.FindByEmailAsync("producer5@example.com"); // Looks up the seeded Identity user by email address.

            // ----- Seed users and roles -----
            if (producerUser1 == null || producerUser2 == null || producerUser3 == null || producerUser4 == null || producerUser5 == null) // Checks the condition before continuing this setup step.
            {
                throw new Exception("Producer users not found."); // Stops seeding when required seed data is missing.
            }

            if (context.Producers.Any()) // Checks whether producer seed records already exist.
                return; // Skips seeding because the records already exist.

            var producers = new List<Producers> // Starts the list of producer records to seed.
            {
                new Producers // Starts one seeded producer record.
                {
                    // ----- Seed producers -----
                    ProducerName = "Green Valley Produce", // Sets ProducerName for seeding or application setup.
                    ProducerEmail = "greenvalley@farmmail.co.uk", // Sets ProducerEmail for seeding or application setup.
                    ProducerInformation = "Green Valley Produce is a family-owned farm located in the heart of the countryside. We specialize in growing organic fruits and vegetables using sustainable farming practices. Our farm has been in operation for over 50 years, and we take pride in providing fresh, high-quality produce to our customers.", // Sets ProducerInformation for seeding or application setup.
                    // ----- Seed users and roles -----
                    UserId = producerUser1.Id, // Sets UserId for seeding or application setup.
                    // ----- Seed producers -----
                    VATNumber = "GB863574975", // Sets VATNumber for seeding or application setup.
                    IsVATRegistered = true, // Sets IsVATRegistered for seeding or application setup.
                    ImagePath = "/images/producers/greenValleyProduce.jpg" // Sets ImagePath for seeding or application setup.
                },

                new Producers // Starts one seeded producer record.
                {
                    ProducerName = "Sunny Brook Farm", // Sets ProducerName for seeding or application setup.
                    ProducerEmail = "sunnybrook@ruralmail.co.uk", // Sets ProducerEmail for seeding or application setup.
                    ProducerInformation = "Sunny Brook Farm is a small-scale farm that focuses on raising free-range poultry and producing farm-fresh eggs. We are committed to animal welfare and sustainable farming methods. Our chickens roam freely in spacious pastures, and we provide them with a nutritious diet to ensure the highest quality eggs for our customers.", // Sets ProducerInformation for seeding or application setup.
                    // ----- Seed users and roles -----
                    UserId = producerUser2.Id, // Sets UserId for seeding or application setup.
                    // ----- Seed producers -----
                    VATNumber = "GB763906214", // Sets VATNumber for seeding or application setup.
                    IsVATRegistered = true, // Sets IsVATRegistered for seeding or application setup.
                    ImagePath = "/images/producers/sunnybrookFarm.jpg" // Sets ImagePath for seeding or application setup.
                },

                new Producers // Starts one seeded producer record.
                {
                    ProducerName = "Oakridge Harvest Hub", // Sets ProducerName for seeding or application setup.
                    ProducerEmail = "oakridge@harvesthub.co.uk", // Sets ProducerEmail for seeding or application setup.
                    ProducerInformation = "Oakridge Harvest Hub is a cooperative of local farmers who come together to share resources and market their produce. We grow a wide variety of fruits, vegetables, and herbs using organic and regenerative farming techniques. Our mission is to support local agriculture and provide fresh, seasonal produce to our community.", // Sets ProducerInformation for seeding or application setup.
                    // ----- Seed users and roles -----
                    UserId = producerUser3.Id, // Sets UserId for seeding or application setup.
                    // ----- Seed producers -----
                    VATNumber = "GB846385085", // Sets VATNumber for seeding or application setup.
                    IsVATRegistered = true, // Sets IsVATRegistered for seeding or application setup.
                    ImagePath = "/images/producers/oakridgeHarvestHub.jpg" // Sets ImagePath for seeding or application setup.

                },

                new Producers // Starts one seeded producer record.
                {
                    ProducerName = "Meadow Fresh Organic Farms", // Sets ProducerName for seeding or application setup.
                    ProducerEmail = "meadowfresh@agromail.co.uk", // Sets ProducerEmail for seeding or application setup.
                    ProducerInformation = "Meadow Fresh Organic Farms is dedicated to producing high-quality organic dairy products", // Sets ProducerInformation for seeding or application setup.
                    // ----- Seed users and roles -----
                    UserId = producerUser4.Id, // Sets UserId for seeding or application setup.
                    // ----- Seed producers -----
                    IsVATRegistered = false, // Sets IsVATRegistered for seeding or application setup.
                    ImagePath = "/images/producers/meadowFreshOrganicFarm.jpg" // Sets ImagePath for seeding or application setup.
                },

                new Producers // Starts one seeded producer record.
                {
                    ProducerName = "Willowcreek Farm", // Sets ProducerName for seeding or application setup.
                    ProducerEmail = "willowcreek@farmconnect.co.uk", // Sets ProducerEmail for seeding or application setup.
                    ProducerInformation = "Willowcreek Farm is a family-run farm that specializes in growing heirloom vegetables and artisanal herbs. We are passionate about preserving traditional farming methods and cultivating unique, flavorful produce. Our farm is committed to sustainability and biodiversity, and we strive to create a thriving ecosystem on our land.", // Sets ProducerInformation for seeding or application setup.
                    // ----- Seed users and roles -----
                    UserId = producerUser5.Id, // Sets UserId for seeding or application setup.
                    // ----- Seed producers -----
                    IsVATRegistered = false, // Sets IsVATRegistered for seeding or application setup.
                    ImagePath = "/images/producers/willowcreekFarm.jpg" // Sets ImagePath for seeding or application setup.
                }
            };

            context.Producers.AddRange(producers); // Queues all seeded producer records for insertion.
            await context.SaveChangesAsync(); // Persists the pending seed or model changes.
        }

        // ----- Seed products -----
        public static async Task SeedProducts(IServiceProvider serviceProvider) // Seeds catalogue products after producer records exist.
        {
            // ----- Database configuration -----
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Resolves a required service from dependency injection.


            // ----- Seed producers -----
            var GreenValleyProducer = await context.Producers.FirstOrDefaultAsync(x => x.ProducerName == "Green Valley Produce"); // Runs this setup or seeding step.
            var SunnyBrookFarm = await context.Producers.FirstOrDefaultAsync(x => x.ProducerName == "Sunny Brook Farm"); // Runs this setup or seeding step.
            var OakridgeHarvestHub = await context.Producers.FirstOrDefaultAsync(x => x.ProducerName == "Oakridge Harvest Hub"); // Runs this setup or seeding step.
            var MeadowFreshOrganicFarms = await context.Producers.FirstOrDefaultAsync(x => x.ProducerName == "Meadow Fresh Organic Farms"); // Runs this setup or seeding step.
            var WillowcreekFarm = await context.Producers.FirstOrDefaultAsync(x => x.ProducerName == "Willowcreek Farm"); // Runs this setup or seeding step.

            if (GreenValleyProducer == null || SunnyBrookFarm == null || OakridgeHarvestHub == null || MeadowFreshOrganicFarms == null || WillowcreekFarm == null) // Checks the condition before continuing this setup step.
            {
                throw new Exception("Producers not found."); // Stops seeding when required seed data is missing.
            }

            if (!context.Products.Any()) // Checks whether product seed records already exist.
            {
                var products = new List<Products> // Starts the list of product records to seed.
                {
                    new Products // Starts one seeded product record.
                    {
                        // ----- Seed products -----
                        ItemName = "Apple", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 100, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.00m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our crisp and juicy apples are handpicked at the peak of ripeness to ensure maximum flavor and freshness. Grown with care on our farm, these apples are perfect for snacking, baking, or adding to salads. Each bite offers a deliciously sweet and slightly tart flavor that will delight your taste buds.", // Sets Description for seeding or application setup.
                        Category = "Fruit", // Sets Category for seeding or application setup.
                        ProducersId = GreenValleyProducer.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/apple.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.
                    },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Broccoli", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 80, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.40m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our fresh and crisp broccoli is perfect for roasting, steaming, or adding to stir-fries. Grown with care on our farm, this broccoli is packed with nutrients and has a deliciously mild flavor that pairs well with a variety of dishes.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = GreenValleyProducer.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/broccoli.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.
                    },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Carrot", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 140, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.70m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our sweet and crunchy carrots are perfect for snacking or adding to salads. Grown with care on our farm, these carrots are harvested at the peak of freshness to ensure maximum flavor and quality.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = GreenValleyProducer.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/carrot.jpg", // Sets ImagePath for seeding or application setup.
                        Available = false // Sets Available for seeding or application setup.

                    },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Cauliflower", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 60, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.80m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our fresh and tender cauliflower is perfect for roasting, steaming, or adding to soups and stews. Grown with care on our farm, this cauliflower is harvested at the peak of freshness to ensure maximum flavor and quality.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = GreenValleyProducer.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/cauliflower.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.
                    },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Potato", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 120, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.45m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our farm-fresh potatoes are harvested at the peak of freshness to ensure maximum flavor and quality. Grown with care on our farm, these potatoes are perfect for roasting, mashing, or frying. They have a deliciously earthy flavor and a fluffy texture that will elevate any dish.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = GreenValleyProducer.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/potatoes.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                    },
                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Free-Range Eggs", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 120, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.50m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our farm-fresh free-range eggs are produced by happy, healthy chickens that roam freely in spacious pastures. These eggs are rich in flavor and packed with nutrients, making them a delicious and wholesome choice for your meals. Whether you're baking, frying, or scrambling, our free-range eggs will add a fresh and wholesome touch to your dishes.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = SunnyBrookFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/egg.jpg", // Sets ImagePath for seeding or application setup.
                        Available = false, // Sets Available for seeding or application setup.
                        Allergens = "Eggs" // Sets Allergens for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Chicken breast", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 65, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 4.80m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our farm-fresh chicken breast is sourced from free-range chickens that are raised on a diet of natural feed and have access to the outdoors. The result is tender, juicy chicken with a deliciously fresh taste. Our commitment to sustainable farming practices means that you can enjoy our chicken breast with confidence, knowing that it is not only good for you but also good for the environment.", // Sets Description for seeding or application setup.
                        Category = "Meat", // Sets Category for seeding or application setup.
                        ProducersId = SunnyBrookFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/chickenbreast.jpg", // Sets ImagePath for seeding or application setup.
                        Available = false // Sets Available for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Cheese", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 90, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 3.80m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our farm-fresh cheese is crafted with care using milk from our grass-fed cows. It has a rich, creamy texture and a deliciously tangy flavor that pairs perfectly with crackers, bread, or fresh fruit. Our cheese is made using traditional methods and is free from artificial additives, ensuring a wholesome and natural taste in every bite.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = SunnyBrookFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/cheese.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Dairy" // Sets Allergens for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Yogurt", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 75, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.10m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our farm-fresh yogurt is made from the freshest milk produced by our happy, grass-fed cows. It is creamy, rich, and packed with probiotics that promote a healthy gut. Our yogurt is available in a variety of delicious flavors, all made with natural ingredients and no artificial additives. Whether you enjoy it on its own, with fruit, or as a base for smoothies and desserts, our farm-fresh yogurt is a wholesome and tasty choice for your daily diet.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = SunnyBrookFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/yogurt.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Dairy" // Sets Allergens for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Milk", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 90, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.20m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our farm-fresh milk is sourced from our happy, grass-fed cows that graze on lush pastures. It is creamy, rich, and packed with nutrients, making it a delicious and wholesome choice for your daily diet. Our commitment to sustainable farming practices ensures that our milk is not only good for you but also good for the environment.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = SunnyBrookFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/milk.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Dairy" // Sets Allergens for seeding or application setup.

                     },


                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Strawberry", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 70, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 2.00m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our sweet and juicy strawberries are handpicked at the peak of ripeness to ensure maximum flavor and freshness. Grown with care on our farm, these strawberries are perfect for snacking, adding to desserts, or blending into smoothies. Each bite bursts with a deliciously sweet and slightly tart flavor that will delight your taste buds.", // Sets Description for seeding or application setup.
                        Category = "Fruit", // Sets Category for seeding or application setup.
                        ProducersId = OakridgeHarvestHub.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/strawberry.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Apple", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 100, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.85m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our crisp and juicy apples are handpicked at the peak of ripeness to ensure maximum flavor and freshness. Grown with care on our farm, these apples are perfect for snacking, baking, or adding to salads. Each bite offers a deliciously sweet and slightly tart flavor that will delight your taste buds.", // Sets Description for seeding or application setup.
                        Category = "Fruit", // Sets Category for seeding or application setup.
                        ProducersId = OakridgeHarvestHub.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/apple.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Pear", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 80, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.80m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our juicy and sweet pears are handpicked at the peak of ripeness to ensure maximum flavor and freshness. Grown with care on our farm, these pears are perfect for snacking or adding to salads. Each bite offers a deliciously sweet and slightly tart flavor that will delight your taste buds.", // Sets Description for seeding or application setup.
                        Category = "Fruit", // Sets Category for seeding or application setup.
                        ProducersId = OakridgeHarvestHub.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/pear.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Raspberry", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 55, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 2.30m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our sweet and juicy raspberries are handpicked at the peak of ripeness to ensure maximum flavor and freshness. Grown with care on our farm, these raspberries are perfect for snacking, adding to desserts, or blending into smoothies. Each bite bursts with a deliciously sweet and slightly tart flavor that will delight your taste buds.", // Sets Description for seeding or application setup.
                        Category = "Fruit", // Sets Category for seeding or application setup.
                        ProducersId = OakridgeHarvestHub.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/raspberry.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Broccoli", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 85, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.10m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our fresh and crisp broccoli is perfect for roasting, steaming, or adding to stir-fries. Grown with care on our farm, this broccoli is packed with nutrients and has a deliciously mild flavor that pairs well with a variety of dishes.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = OakridgeHarvestHub.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/broccoli.jpg", // Sets ImagePath for seeding or application setup.
                        Available = false // Sets Available for seeding or application setup.
                     },


                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Organic Milk", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 95, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.30m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our organic milk is sourced from cows that are raised on lush, green pastures and fed a natural diet free from synthetic hormones and antibiotics. The result is a creamy, rich milk with a deliciously fresh taste that you can feel good about. Our commitment to sustainable farming practices ensures that our milk is not only good for you but also good for the environment.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = MeadowFreshOrganicFarms.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/milk.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Dairy" // Sets Allergens for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Cheese", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 50, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 4.20m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our organic cheese is crafted with care using milk from our grass-fed cows. It has a rich, creamy texture and a deliciously tangy flavor that pairs perfectly with crackers, bread, or fresh fruit. Our cheese is made using traditional methods and is free from artificial additives, ensuring a wholesome and natural taste in every bite.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = MeadowFreshOrganicFarms.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/cheese.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Dairy" // Sets Allergens for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Yogurt", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 70, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.30m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our organic yogurt is made from the freshest milk produced by our happy, grass-fed cows. It is creamy, rich, and packed with probiotics that promote a healthy gut. Our yogurt is available in a variety of delicious flavors, all made with natural ingredients and no artificial additives. Whether you enjoy it on its own, with fruit, or as a base for smoothies and desserts, our organic yogurt is a wholesome and tasty choice for your daily diet.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = MeadowFreshOrganicFarms.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/yogurt.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Dairy" // Sets Allergens for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Egg", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 100, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.40m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our organic eggs are produced by free-range chickens that are raised on a diet of organic feed and have access to the outdoors. These eggs are rich in flavor and packed with nutrients, making them a healthy and delicious choice for your meals. Whether you're baking, frying, or scrambling, our organic eggs will add a fresh and wholesome touch to your dishes.", // Sets Description for seeding or application setup.
                        Category = "Dairy", // Sets Category for seeding or application setup.
                        ProducersId = MeadowFreshOrganicFarms.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/egg.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true, // Sets Available for seeding or application setup.
                        Allergens = "Eggs" // Sets Allergens for seeding or application setup.
                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Mince Meat", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 55, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 5.50m, // Sets ItemPrice for seeding or application setup.
                        Description = "Our organic mince meat is made from high-quality, grass-fed beef that is raised without the use of antibiotics or hormones. It is carefully ground to ensure a consistent texture and is perfect for making delicious meals like burgers, meatballs, and spaghetti bolognese. Our commitment to sustainable farming practices means that you can enjoy our mince meat with confidence, knowing that it is not only good for you but also good for the environment.", // Sets Description for seeding or application setup.
                        Category = "Meat", // Sets Category for seeding or application setup.
                        ProducersId = MeadowFreshOrganicFarms.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/mincemeat.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.
                     },


                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Carrot", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 130, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.75m, // Sets ItemPrice for seeding or application setup.
                        Description = "A sweet and crunchy carrot, perfect for snacking or adding to salads. Grown with care on our farm, these carrots are harvested at the peak of freshness to ensure maximum flavor and quality.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = WillowcreekFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/carrot.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Cauliflower", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 65, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.70m, // Sets ItemPrice for seeding or application setup.
                        Description = "A fresh and tender cauliflower, perfect for roasting, steaming, or adding to soups and stews. Grown with care on our farm, this cauliflower is harvested at the peak of freshness to ensure maximum flavor and quality.", // Sets Description for seeding or application setup.
                        Category =  "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = WillowcreekFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/cauliflower.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Potato", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 110, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.40m, // Sets ItemPrice for seeding or application setup.
                        Description = "A versatile and hearty potato, perfect for roasting, mashing, or frying. Grown with care on our farm, these potatoes are harvested at the peak of freshness to ensure maximum flavor and quality.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = WillowcreekFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/potatoes.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Broccoli", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 75, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 1.30m, // Sets ItemPrice for seeding or application setup.
                        Description = "A fresh and crisp broccoli, perfect for roasting, steaming, or adding to stir-fries. Grown with care on our farm, this broccoli is packed with nutrients and has a deliciously mild flavor that pairs well with a variety of dishes.", // Sets Description for seeding or application setup.
                        Category = "Vegetable", // Sets Category for seeding or application setup.
                        ProducersId = WillowcreekFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/broccoli.jpg", // Sets ImagePath for seeding or application setup.
                        Available = true // Sets Available for seeding or application setup.

                     },

                    new Products // Starts one seeded product record.
                    {
                        ItemName = "Pear", // Sets ItemName for seeding or application setup.
                        QuantityInStock = 70, // Sets QuantityInStock for seeding or application setup.
                        ItemPrice = 0.90m, // Sets ItemPrice for seeding or application setup.
                        Description = "A juicy and sweet pear, perfect for snacking or adding to salads. Grown with care on our farm, these pears are harvested at the peak of ripeness to ensure maximum flavor and freshness.", // Sets Description for seeding or application setup.
                        Category = "Fruit", // Sets Category for seeding or application setup.
                        ProducersId = WillowcreekFarm.ProducersId, // Sets ProducersId for seeding or application setup.
                        ImagePath = "/images/products/pear.jpg", // Sets ImagePath for seeding or application setup.
                        Available = false // Sets Available for seeding or application setup.
                     },


                };
                context.Products.AddRange(products); // Queues all seeded product records for insertion.
                await context.SaveChangesAsync(); // Persists the pending seed or model changes.
            }
        }
    }
}
