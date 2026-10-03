import os
import re

directory = r"D:\.Net Projects\MScIT\Ecommerce_Website_MVC\Views"

pattern = re.compile(r'(\{\s*)@(foreach|if|using|switch|for|while)\b')

for root, _, files in os.walk(directory):
    for f in files:
        if f.endswith('.cshtml'):
            path = os.path.join(root, f)
            with open(path, 'r', encoding='utf-8') as file:
                content = file.read()
            
            new_content = pattern.sub(r'\1\2', content)
            
            if new_content != content:
                with open(path, 'w', encoding='utf-8') as file:
                    file.write(new_content)
                print(f"Fixed nested Razor @ in {f}")
