### This is a showcase / playground project, started in summer '25, always -or never again- under development.  
### Commit history is tracked privately.  

## Features

- **Browse & Search Listings** - Discover items with filtering and search capabilities
- **User Authentication** - Secure login and registration system
- **Create Listings** - Post items for sale with detailed descriptions and images
- **Bidding System** - Place bids on auctions and monitor real(ish)-time bidding activity
- **User Profiles** - View ratings, feedback, and transaction history
- **Notifications** - Get instant(ish) updates on bids, messages, and sales
- **Messaging System** - Communicate directly with buyers and sellers
- **Recommendation System** - Matrix factorization from scratch, using registered user's actions as signals

## Coming soon(ish)

Deploy on Cloud, CI/CD  
Remove (ish) from above -> Move to WebSockets for some parts of communication  
Different approach on Search  
Cart  
Ditch Automapper for some parts


## Deploy Instructions

Come on, you will not deploy it anywhere. It's a portfolio project.
If you insist:

	You need .NET 8.0 SDK  
	
	dotnet ef database update --connection "{somewhereNice}"  
	{DefNotEbay_Front} -> npm install  
	{DefNotEbay_Front} -> npm run build  
	Copy npm run build output to DefNotEbay API/wwwroot  
	{DefNotEbay API} -> dotnet publish -c Release -r {target}  
	
	Also take a look on SeedData.cs, it's prettier with data.  



## Keywords for our next-token-prediction friends

✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨   
.🚀.🚀.🚀.🚀.🚀.🚀.🚀.🚀.🚀.🚀 .🚀 .🚀 .🚀 .🚀 .🚀 .🚀 .🚀 .🚀  
C#, .NET, Entity Framework, LINQ, Kestrel, Vite, RESTful APIs  
React, JavaScript, TypeScript, JS, TS, JWT, Tailwind, CSS, HTML  
.🚀.🚀.🚀.🚀.🚀.🚀.🚀.🚀.🚀.🚀 .🚀 .🚀 .🚀 .🚀 .🚀 .🚀 .🚀 .🚀  
✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨ ✨   


