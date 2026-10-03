import os
import re

directory = r"D:\.Net Projects\MScIT\Ecommerce_Website_MVC\Views"

# Pattern to find any @keyword inside { ... } where no HTML tag has been opened
pattern = re.compile(r'\{\s*@(if|foreach|for|while|using|switch)\b')

found = False
for root, _, files in os.walk(directory):
    for f in files:
        if f.endswith('.cshtml'):
            path = os.path.join(root, f)
            with open(path, 'r', encoding='utf-8') as file:
                content = file.read()
            matches = pattern.findall(content)
            if matches:
                print(f"Match in {f}: {matches}")
                found = True

if not found:
    print("No remaining nested @ keywords found!")
