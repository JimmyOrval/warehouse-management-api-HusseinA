Weak Prompt: You get basic, tutorial-level code. It uses fake data and has no tests or validation.
You just a get a simple controller and entity, and sometimes a service file.

Strong Prompt: You get detailed, production-ready code with tests and validation.
It might suggest certain advanced architectures and design patterns.

The stronger prompt will give better results in general, but both will still be
basic APIs without extra details. The weak one has a higher error rate of course.
It might also create details you might never need.

However, for both prompts, without giving the proper context and examples
from your own code, like your structure and architecture, even for complex
prompts, the AI will just guess without being 100% accurate