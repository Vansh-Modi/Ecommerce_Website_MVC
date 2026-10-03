import os
import re

directory = r"D:\.Net Projects\MScIT\Ecommerce_Website_MVC\Views"

for root, _, files in os.walk(directory):
    for f in files:
        if f.endswith('.cshtml'):
            path = os.path.join(root, f)
            with open(path, 'r', encoding='utf-8', errors='ignore') as file:
                content = file.read()
            
            changed = False
            
            # Passwords
            new_content = re.sub(r'placeholder="[^a-zA-Z0-9\s"]+"', 'placeholder="********"', content)
            if new_content != content:
                content = new_content
                changed = True

            # Rupee
            replacements = {
                "'": "Rs.",
                "?""": "-",
                "o\"": "✓",
                "o ": "❌",
                "oZ": "Edit",
                "? Back": "← Back",
                "'</a>": "→</a>",
                "'</button>": "→</button>",
                "s?": "⚠️",
                "^'": "x",
                "o.": "🎉",
                "-": "▼",
                "-": "x"
            }
            
            for k, v in replacements.items():
                if k in content:
                    content = content.replace(k, v)
                    changed = True
            
            if changed:
                with open(path, 'w', encoding='utf-8') as file:
                    file.write(content)
                print(f"Fixed {f}")
