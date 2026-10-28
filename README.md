# LocalFit

Local calorie, food, and weight tracker. Data stays in a SQLite file under Local Application Data.

Avalonia, CommunityToolkit.Mvvm, EF Core. .NET 10. Desktop, Android, iOS, and browser.

```
dotnet run --project LocalFit.Desktop
```

## Todo

- [x] Dialogs for adding and editing data
- [ ] Barcode scanner for presets
- [ ] Camera and photos
- [ ] Food API to seed a base database
- [ ] Move view-specific fields out of models
- [x] Black and white UI (`#222222` / `#DDDDDD`)
- [ ] [shad-ui](https://github.com/accntech/shad-ui) — adopt, or too little difference?
- [x] Favourite tracking (`UseCount` on presets)
- [x] Default new entries to the last used amount
- [ ] Date picker for quick navigation
- [ ] Display sugar
- [ ] Water tracking
- [ ] Charts for weight and BMI
- [ ] Confirm before delete, and restore soft-deleted items (cascading deletes can break related data)

## Todo Global

- Translations
- Food backend
- User backups (possible paid feature)
- Social features (friends, groups, challenges) — unclear if useful
- LLM integration — unclear if useful
- Meal photos
