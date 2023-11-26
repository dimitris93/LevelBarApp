# Case 2: refactoring
This case is about improving the overall quality of the code and implementing simplifications, such that it becomes a lean and mean piece of software. Please keep in mind to make separate commits, which helps in the discussion later.

## Code standards
The original code was created in times of .NET Framework 4.7 running on C# 7.3. As the team has since progressed along with the updates in C# and .NET, this code now reflects a "legacy" way of coding.
* Update the project to .NET Framework 4.8 and the latest C# version
* Refactor some legacy patterns to the preffered way of working of these latest versions 

## Generator
The team decided to phase out the Quartz package that is now used for the scheduled job.
* Please replace the Quartz job by something that uses a standard C# TPL pattern.
* Add a tests to verify correct behaviour of the job.

## MVVM
* Refactor the static instance of the generator to be non-static, using a more professional pattern (like IoC).
* The code used to define commands is quite verbose. Please try to simplify.
