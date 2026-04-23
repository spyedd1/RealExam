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

            string[] producerSeedEmails =
            {
                "producer@example.com",
                "producer2@example.com",
                "producer3@example.com",
                "producer4@example.com",
                "producer5@example.com"
            };

            string[] standardSeedEmails =
            {
                "user@example.com",
                "user2@example.com",
                "user3@example.com",
                "user4@example.com",
                "user5@example.com"
            };

            string[] privilegedSeedEmails =
            {
                "admin@example.com",
                "developer@example.com"
            };

            foreach (var email in producerSeedEmails) // Ensures the producer logins exist for seeded producer profiles.
            {
                var producerUser = await userManager.FindByEmailAsync(email); // Looks up the seeded Identity user by email address.
                if (producerUser == null) // Checks the condition before continuing this setup step.
                {
                    producerUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true }; // Creates the Identity user object for seeding.
                    await userManager.CreateAsync(producerUser, "Password123!"); // Creates the seeded user with the default test password.
                }

                if (!await userManager.IsInRoleAsync(producerUser, "Producer")) // Checks whether the seeded user already has the role.
                {
                    await userManager.AddToRoleAsync(producerUser, "Producer"); // Assigns the required role to the seeded user.
                }
            }

            foreach (var email in standardSeedEmails) // Creates the five standard user accounts requested for seeded orders.
            {
                var standardUser = await userManager.FindByEmailAsync(email); // Looks up the seeded Identity user by email address.
                if (standardUser == null) // Checks the condition before continuing this setup step.
                {
                    standardUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true }; // Creates the Identity user object for seeding.
                    await userManager.CreateAsync(standardUser, "Password123!"); // Creates the seeded user with the default test password.
                }

                if (!await userManager.IsInRoleAsync(standardUser, "Standard")) // Checks whether the seeded user already has the role.
                {
                    await userManager.AddToRoleAsync(standardUser, "Standard"); // Assigns the required role to the seeded user.
                }
            }

            foreach (var email in privilegedSeedEmails) // Restores the admin and developer seed accounts.
            {
                var privilegedUser = await userManager.FindByEmailAsync(email); // Looks up the seeded Identity user by email address.
                if (privilegedUser == null) // Checks the condition before continuing this setup step.
                {
                    privilegedUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true }; // Creates the Identity user object for seeding.
                    await userManager.CreateAsync(privilegedUser, "Password123!"); // Creates the seeded user with the default test password.
                }

                var requiredRole = email == "admin@example.com" ? "Admin" : "Developer"; // Maps each privileged account to its application role.
                if (!await userManager.IsInRoleAsync(privilegedUser, requiredRole)) // Checks whether the seeded user already has the role.
                {
                    await userManager.AddToRoleAsync(privilegedUser, requiredRole); // Assigns the required role to the seeded user.
                }
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

        // ----- Seed orders -----
        public static async Task SeedOrders(IServiceProvider serviceProvider) // Seeds test orders for the five seeded users.
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>(); // Resolves a required service from dependency injection.
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>(); // Resolves a required service from dependency injection.

            if (await context.Orders.AnyAsync()) // Checks whether order seed records already exist.
                return; // Skips seeding because the records already exist.

            string[] standardSeedEmails =
            {
                "user@example.com",
                "user2@example.com",
                "user3@example.com",
                "user4@example.com",
                "user5@example.com"
            };

            var seededUsers = new List<IdentityUser>(); // Collects the five seeded users in a fixed order for predictable order counts.
            foreach (var email in standardSeedEmails) // Loads the users that will own the seeded orders.
            {
                var user = await userManager.FindByEmailAsync(email); // Looks up the seeded Identity user by email address.
                if (user == null) // Checks the condition before continuing this setup step.
                {
                    throw new Exception($"Seed user '{email}' not found."); // Stops seeding when required seed data is missing.
                }

                seededUsers.Add(user); // Tracks the user for the later order seeding loop.
            }

            var products = await context.Products // Loads product data needed to build realistic seeded orders.
                .Include(x => x.Producers) // Includes producer data for producer order splitting and invoice generation.
                .Where(x => x.Available) // Filters to products that can be used in seeded orders.
                .OrderBy(x => x.ProductsId) // Keeps product selection stable across runs.
                .ToListAsync(); // Materializes the query so products can be reused.

            if (products.Count == 0) // Checks the condition before continuing this setup step.
            {
                throw new Exception("Products not found for order seeding."); // Stops seeding when required seed data is missing.
            }

            for (int userIndex = 0; userIndex < seededUsers.Count; userIndex++) // Creates 0, 1, 2, 3, and 4 orders for users 1 through 5.
            {
                var user = seededUsers[userIndex]; // Gets the current seed user for this batch of orders.
                int orderCount = userIndex; // Maps user 1 to 0 orders, user 2 to 1 order, and so on.

                for (int orderNumber = 0; orderNumber < orderCount; orderNumber++) // Creates the required number of orders for the current user.
                {
                    var selectedProduct = products[(userIndex + orderNumber) % products.Count]; // Reuses seeded products in a predictable cycle.
                    decimal orderTotal = selectedProduct.ItemPrice; // Calculates the total for this single-line seeded order.
                    bool isDeliveryOrder = orderNumber % 2 == 0; // Alternates delivery and collection orders to vary the seed data.
                    var orderDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-(userIndex + orderNumber + 1))); // Spreads seeded orders across recent dates.

                    var order = new Orders // Creates the parent order record used by the following workflow.
                    {
                        UserId = user.Id, // Sets UserId for seeding or application setup.
                        OrderDate = orderDate, // Sets OrderDate for seeding or application setup.
                        DeliveryMethod = isDeliveryOrder ? "Standard" : "Collection", // Sets DeliveryMethod for seeding or application setup.
                        Delivery = isDeliveryOrder, // Sets Delivery for seeding or application setup.
                        Collection = !isDeliveryOrder, // Sets Collection for seeding or application setup.
                        OrdersTotal = orderTotal, // Sets OrdersTotal for seeding or application setup.
                        TrackingStatus = isDeliveryOrder ? "Delivered" : "Collected", // Sets TrackingStatus for seeding or application setup.
                        TermsAccepted = true, // Sets TermsAccepted for seeding or application setup.
                        DateOfCollection = isDeliveryOrder ? null : orderDate.AddDays(2), // Sets DateOfCollection for seeding or application setup.
                        OrderStatus = "Accepted", // Sets OrderStatus for seeding or application setup.
                        DeliveryConfirmed = isDeliveryOrder, // Sets DeliveryConfirmed for seeding or application setup.
                        BillingLine1 = $"{userIndex + 1} Market Street", // Sets BillingLine1 for seeding or application setup.
                        BillingLine2 = null, // Sets BillingLine2 for seeding or application setup.
                        BillingCity = "Bristol", // Sets BillingCity for seeding or application setup.
                        BillingPostcode = $"BS1 {userIndex}{orderNumber}AA", // Sets BillingPostcode for seeding or application setup.
                        DeliveryLine1 = isDeliveryOrder ? $"{userIndex + 1} Market Street" : null, // Sets DeliveryLine1 for seeding or application setup.
                        DeliveryLine2 = null, // Sets DeliveryLine2 for seeding or application setup.
                        DeliveryCity = isDeliveryOrder ? "Bristol" : null, // Sets DeliveryCity for seeding or application setup.
                        DeliveryPostcode = isDeliveryOrder ? $"BS1 {userIndex}{orderNumber}AA" : null // Sets DeliveryPostcode for seeding or application setup.
                    };

                    context.Orders.Add(order); // Queues the new entity to be inserted into the database.
                    await context.SaveChangesAsync(); // Persists pending database changes asynchronously.

                    var producerOrder = new ProducerOrders // Creates the producer order slice for the seeded order.
                    {
                        OrdersId = order.OrdersId, // Sets OrdersId for seeding or application setup.
                        ProducerId = selectedProduct.Producers.UserId, // Sets ProducerId for seeding or application setup.
                        ProducerSubtotal = orderTotal, // Sets ProducerSubtotal for seeding or application setup.
                        TrackingStatus = "Accepted" // Sets TrackingStatus for seeding or application setup.
                    };

                    context.ProducerOrders.Add(producerOrder); // Queues the new entity to be inserted into the database.
                    await context.SaveChangesAsync(); // Persists pending database changes asynchronously.

                    context.OrderProducts.Add(new OrderProducts // Creates the order line item for the seeded order.
                    {
                        OrdersId = order.OrdersId, // Sets OrdersId for seeding or application setup.
                        ProductsId = selectedProduct.ProductsId, // Sets ProductsId for seeding or application setup.
                        ProductQuantity = 1, // Sets ProductQuantity for seeding or application setup.
                        ProducerOrdersId = producerOrder.ProducerOrdersId, // Sets ProducerOrdersId for seeding or application setup.
                        InvoiceNumber = selectedProduct.Producers.IsVATRegistered // Creates invoice numbers only for VAT-registered producers.
                            ? $"INV-{order.OrderDate:yyyyMMdd}-{order.OrdersId:D6}-{selectedProduct.Producers.ProducersId}"
                            : null // Stores null when the preceding condition is false.
                    });
                }
            }

            await context.SaveChangesAsync(); // Persists the pending seed or model changes.
        }
    }
}
