# Backend AI Report

## Best Prompt:
`Exercise 1 (expiring-soon endpoint)`

I gave it the ProductsController, ProductRepository, and
ProductViewModel before asking for anything. Output matched my real architecture,
and barely needed fixing.

## Best Generated Code:
`Exercise 7 (shipment module)`

Small and matched my CQRS/MediatR setup cleanly.
Telling it to keep it minimal helped from overbuilding.

## Worst/Most Dangerous Generated Code:
`Exercise 5`

Since I gave it a big repository, a small change anwhere, unfitting of my architecture
could have broken my entire API.

## Other Incorrect Stuff Caught:
- `Exercise 1`: It used DateTime.Now instead of UtcNow,
  and the date filter only had an upper bound so
  already-expired products would've shown as "expiring soon"


- `Exercise 2`: It tried to fix a bug that didn't actually exist
  (archived product check was already there),
  especially since I just gave it the prompt as-is.


## Human Corrections Made:
- Fixed the date logic + timezone bug
- Chose where to add AsNoTracking in repository
- Added my own mock helper methods in tests
- Checked all code it provided to make sure it fit my architecture

## Lessons Learned:
- Giving it the context is better than any prompt you can create
- Don't make the required changes exactly as asked, you might already have them in your code
- AI can generate code for something that doesn't apply just because you mentioned
  it, instead of trusting completely, you have to check it that manually