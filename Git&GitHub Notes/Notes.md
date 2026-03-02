# 🚀 Git & GitHub — The Complete Developer Bible

> *Everything you'll ever need. From zero to hero. Bookmark this.*

---

## 📚 Table of Contents

1. [What is Git? What is GitHub?](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-1-what-is-git-what-is-github)
2. [How Git Works Internally](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-2-how-git-works-internally)
3. [Setup &amp; Installation](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-3-setup--installation)
4. [The Git Workflow — The Core Flow](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-4-the-git-workflow--the-core-flow)
5. [Basic Commands](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-5-basic-commands)
6. [Branching &amp; Merging](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-6-branching--merging)
7. [Working with Remote Repositories](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-7-working-with-remote-repositories)
8. [Stashing &amp; Cleaning](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-8-stashing--cleaning)
9. [Tagging](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-9-tagging)
10. [Undoing Things — The Rescue Chapter](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-10-undoing-things--the-rescue-chapter)
11. [Advanced Commands](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-11-advanced-commands)
12. [GitHub-Specific Features](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-12-github-specific-features)
13. [VSCode + Git Setup](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-13-vscode--git-setup)
14. [Real-World Workflows](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-14-real-world-workflows)
15. [Common Issues &amp; Fixes](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-15-common-issues--fixes)
16. [Quick Reference Cheatsheet](https://claude.ai/chat/ae113d7d-c1ef-4881-9966-3f034c6894bd#chapter-16-quick-reference-cheatsheet)

---

# Chapter 1: What is Git? What is GitHub?

## 🔷 Git

**Git** is a **version control system** — a tool that tracks every change you make to your code over time.

Think of it like this:

> Imagine you are writing a novel. Every time you finish a chapter, you save a copy of the whole book. If you mess up chapter 10, you can go back to the copy you saved after chapter 9. **Git does exactly this for your code — automatically, efficiently, and with full history.**

* Git was created by **Linus Torvalds** in 2005 (the same person who created Linux).
* It works **locally on your computer** — no internet needed.
* It stores your entire project history in a hidden folder called `.git`.

## 🔷 GitHub

**GitHub** is a **cloud platform** that hosts your Git repositories online.

Think of it like this:

> Git is like your personal diary (on your desk). GitHub is like a Google Drive for that diary — you can access it from anywhere, share it with others, and collaborate.

* GitHub is **not** Git. GitHub uses Git underneath.
* Other alternatives: GitLab, Bitbucket — but GitHub is the most popular.
* GitHub adds features: Pull Requests, Issues, Actions (CI/CD), GitHub Pages, etc.

## 🔷 Why Do Developers Use Git?

| Problem without Git                | Solution with Git                    |
| ---------------------------------- | ------------------------------------ |
| "Oops, I deleted my code"          | Go back to any previous version      |
| "My feature broke everything"      | Roll back to before your changes     |
| "Two devs edited the same file"    | Git handles merging automatically    |
| "What changed and who changed it?" | `git log`and `git blame`tell you |
| "I want to try something risky"    | Create a branch — experiment safely |

---

# Chapter 2: How Git Works Internally

## 🔷 The 4 Areas of Git

This is the most important concept. Everything in Git revolves around these 4 areas:

```
┌─────────────────┐     git add      ┌──────────────┐    git commit    ┌───────────────┐    git push    ┌────────────────┐
│                 │ ───────────────► │              │ ───────────────► │               │ ─────────────► │                │
│ Working         │                  │  Staging     │                  │  Local        │                │  Remote        │
│ Directory       │                  │  Area        │                  │  Repository   │                │  Repository    │
│ (your files)    │ ◄─────────────── │  (Index)     │                  │  (.git folder)  ◄───────────── │  (GitHub)      │
│                 │   git checkout   │              │                  │               │   git fetch /  │                │
└─────────────────┘                  └──────────────┘                  └───────────────┘   git pull     └────────────────┘
```

### Area 1: Working Directory

* This is your **actual project folder** on your computer.
* Files here can be **tracked** (Git knows about them) or **untracked** (new files Git hasn't seen).
* When you edit a file, it becomes  **modified** .

### Area 2: Staging Area (Index)

* A **preparation zone** before committing.
* You explicitly choose which changes go into your next commit.
* Analogy: Think of it as a **shopping cart** — you pick what you want before checking out (committing).

### Area 3: Local Repository (.git folder)

* When you commit, changes move here permanently (in your local machine).
* This is where  **all your commit history lives** .
* The `.git` folder inside your project IS the repository.

### Area 4: Remote Repository (GitHub)

* Your code hosted on GitHub (or GitLab, etc.).
* Others can see it, clone it, contribute to it.
* You push to it, pull from it.

## 🔷 What is a Commit?

A **commit** is a snapshot of your project at a point in time. It stores:

* The state of all tracked files
* A unique ID (SHA hash) like `a3f5c2d`
* Author name & email
* Timestamp
* A message describing what changed
* A reference to the previous commit (parent)

Commits form a **chain (linked list)** — this chain is your project's history.

```
[Initial commit] ──► [Add login page] ──► [Fix bug #42] ──► [Add dark mode]  ← HEAD (latest)
    a1b2c3d               e4f5g6h               i7j8k9l              m1n2o3p
```

## 🔷 What is HEAD?

`HEAD` is a **pointer** that tells Git: "This is where you currently are."

* Normally HEAD points to the latest commit of your current branch.
* When you switch branches, HEAD moves.
* When you check out an old commit, HEAD becomes "detached."

## 🔷 What is a Branch?

A **branch** is just a lightweight  **pointer to a commit** . That's it.

```
main:    [C1] ──► [C2] ──► [C3]
                               ↑
                             (main branch pointer)

feature: [C1] ──► [C2] ──► [C3] ──► [C4] ──► [C5]
                                                 ↑
                                        (feature branch pointer)
```

When you create a branch, Git just creates a new pointer — it doesn't copy files. That's why branches are so cheap and fast in Git.

---

# Chapter 3: Setup & Installation

## 🔷 Install Git

**Windows:**

1. Download from https://git-scm.com/download/win
2. Install with default settings
3. Use "Git Bash" as your terminal

**Mac:**

```bash
# Option 1: Install Xcode Command Line Tools (easiest)
xcode-select --install

# Option 2: Use Homebrew
brew install git
```

**Linux (Ubuntu/Debian):**

```bash
sudo apt update
sudo apt install git
```

**Verify installation:**

```bash
git --version
# Output: git version 2.x.x
```

## 🔷 Configure Git (Do This First — Always!)

These settings attach your identity to every commit you make.

```bash
# Set your name
git config --global user.name "Your Full Name"

# Set your email (use same email as GitHub)
git config --global user.email "you@example.com"

# Set default branch name to 'main' (modern standard)
git config --global init.defaultBranch main

# Set your preferred code editor (examples below)
git config --global core.editor "code --wait"     # VSCode
git config --global core.editor "vim"             # Vim
git config --global core.editor "nano"            # Nano

# Make Git output colorful (highly recommended)
git config --global color.ui auto

# View all your settings
git config --list

# View a specific setting
git config user.name
```

**What does `--global` mean?**

* `--global` = applies to ALL git repos on your computer
* `--local` = applies only to the current repo (overrides global)
* `--system` = applies to all users on the computer

## 🔷 Set Up SSH Key for GitHub (Recommended)

SSH lets you push/pull without entering password every time.

```bash
# Step 1: Generate SSH key
ssh-keygen -t ed25519 -C "you@example.com"
# Press Enter for all prompts (or set a passphrase for security)

# Step 2: Start SSH agent
eval "$(ssh-agent -s)"

# Step 3: Add your key to the agent
ssh-add ~/.ssh/id_ed25519

# Step 4: Copy your PUBLIC key
cat ~/.ssh/id_ed25519.pub
# Copy the entire output

# Step 5: Add to GitHub
# Go to GitHub → Settings → SSH and GPG keys → New SSH key
# Paste your key and save

# Step 6: Test the connection
ssh -T git@github.com
# You should see: Hi username! You've successfully authenticated...
```

## 🔷 Set Up a .gitignore

`.gitignore` tells Git which files to  **never track** .

```bash
# Create .gitignore in your project root
touch .gitignore
```

**Common .gitignore entries:**

```gitignore
# Dependencies
node_modules/
vendor/

# Environment files (NEVER commit these — contains secrets!)
.env
.env.local
.env.production

# Build outputs
dist/
build/
*.o
*.exe

# OS files
.DS_Store           # Mac
Thumbs.db           # Windows

# IDE files
.vscode/
.idea/

# Logs
*.log
logs/

# Python
__pycache__/
*.pyc
venv/
```

> 💡 **Pro tip:** Use https://gitignore.io to auto-generate .gitignore for your tech stack.

---

# Chapter 4: The Git Workflow — The Core Flow

## 🔷 The Standard Day-to-Day Flow

This is what you do every single day as a developer:

```
1. PULL latest changes from remote
        ↓
2. CREATE/SWITCH to a branch for your work
        ↓
3. EDIT your files (write code)
        ↓
4. STAGE the files you changed (git add)
        ↓
5. COMMIT your staged changes (git commit)
        ↓
6. PUSH your branch to remote (git push)
        ↓
7. Create a PULL REQUEST on GitHub
        ↓
8. Get code reviewed → MERGE to main
        ↓
9. Repeat from Step 1
```

## 🔷 The 3 States of a File

Every file in your working directory is in one of these states:

```
UNTRACKED → STAGED → COMMITTED → MODIFIED → STAGED → COMMITTED...

New file    git add   git commit  You edit   git add   git commit
(Git         ──────►   ──────►    the file   ──────►   ──────►
doesn't      Stage it  Save it    again      Stage it  Save it
know it)
```

| State                        | Meaning                                            |
| ---------------------------- | -------------------------------------------------- |
| **Untracked**          | New file, Git has never seen it                    |
| **Tracked/Unmodified** | Git knows it, nothing changed since last commit    |
| **Modified**           | Git knows it, you changed it, not yet staged       |
| **Staged**             | Changed and added to staging area, ready to commit |

---

# Chapter 5: Basic Commands

## 🔷 Starting a Repository

```bash
# Initialize a new Git repo in current folder
git init

# Initialize with a specific branch name
git init -b main

# Clone an existing repository from GitHub
git clone https://github.com/username/repo.git

# Clone into a specific folder name
git clone https://github.com/username/repo.git my-folder

# Clone with SSH (after SSH setup)
git clone git@github.com:username/repo.git
```

> After `git clone`, you DON'T need to `git init` — it's already a git repo.

## 🔷 Checking Status

```bash
# See the current state of your working directory and staging area
git status

# Short/compact status view
git status -s
# ?? = untracked, M = modified, A = staged (added)
```

## 🔷 Staging Files (git add)

```bash
# Stage a specific file
git add filename.txt

# Stage multiple files
git add file1.txt file2.txt

# Stage all changes in current directory and subdirectories
git add .

# Stage all changes in the whole repo (from any directory)
git add -A

# Stage parts of a file interactively (pick specific lines/hunks)
git add -p filename.txt

# Stage all modified tracked files (does NOT add new untracked files)
git add -u
```

## 🔷 Committing (git commit)

```bash
# Commit with a message (opens editor if no -m)
git commit -m "Your commit message here"

# Stage all tracked files AND commit in one step
git commit -am "Message"
# ⚠️ This does NOT add untracked (new) files

# Amend (fix) the last commit message or add forgotten files
git commit --amend -m "Corrected message"
# ⚠️ Only do this if you haven't pushed yet!

# Commit with a detailed multi-line message (opens editor)
git commit
```

### ✅ How to Write Good Commit Messages

```
Format:
<type>: <short summary in present tense, max 72 chars>

[optional body - explain WHY, not WHAT]
[optional footer - reference issues]

Examples:
feat: add user authentication
fix: resolve null pointer on login page
docs: update README with setup instructions
style: format code with prettier
refactor: extract payment logic to service
test: add unit tests for Cart component
chore: upgrade dependencies

Types: feat, fix, docs, style, refactor, test, chore, perf
```

## 🔷 Viewing History (git log)

```bash
# Full log
git log

# Compact one-line per commit
git log --oneline

# Show last N commits
git log -5

# Beautiful graph showing branches
git log --oneline --graph --all

# Search commits by author
git log --author="Name"

# Search commits by message keyword
git log --grep="login"

# Show commits that changed a specific file
git log -- filename.txt

# Show commits between dates
git log --after="2024-01-01" --before="2024-06-01"
```

## 🔷 Viewing Changes (git diff)

```bash
# Show changes in working directory (not yet staged)
git diff

# Show changes that ARE staged (ready to commit)
git diff --staged
# or
git diff --cached

# Show changes between two commits
git diff abc123 def456

# Show changes between two branches
git diff main feature-branch

# Show changes in a specific file
git diff filename.txt
```

## 🔷 Viewing Specific Commits (git show)

```bash
# Show the latest commit's changes
git show

# Show a specific commit
git show abc1234

# Show a file at a specific commit
git show abc1234:filename.txt
```

---

# Chapter 6: Branching & Merging

## 🔷 Why Branches?

Branches let you work on features, bugs, experiments  **without affecting the main codebase** .

```
main:    [C1] ──► [C2] ──► [C3] ─────────────────────────────► [C6: Merge]
                               \                                /
feature:                        ──► [C4: new feature] ──► [C5]
```

**Golden Rule:** Never commit directly to `main` in a team. Always use branches.

## 🔷 Branch Commands

```bash
# List all local branches (* = current branch)
git branch

# List all branches including remote
git branch -a

# Create a new branch
git branch feature-login

# Switch to a branch
git checkout feature-login
# Modern way (Git 2.23+):
git switch feature-login

# Create AND switch to new branch (two-in-one)
git checkout -b feature-login
# Modern way:
git switch -c feature-login

# Rename current branch
git branch -m new-name

# Delete a branch (only if merged)
git branch -d feature-login

# Force delete a branch (even if not merged — careful!)
git branch -D feature-login

# See which branches are merged into current
git branch --merged

# See which branches are NOT merged
git branch --no-merged
```

## 🔷 Merging Branches

```bash
# Step 1: Switch to the branch you want to merge INTO
git checkout main

# Step 2: Merge the feature branch into main
git merge feature-login
```

### Types of Merges

**Fast-Forward Merge** (simplest — no conflicts possible):

```
Before:
main:    [C1] ──► [C2] ──► [C3]
                              \
feature:                       ──► [C4] ──► [C5]

After git merge feature:
main:    [C1] ──► [C2] ──► [C3] ──► [C4] ──► [C5]
```

Git just moves the main pointer forward. No new commit created.

**3-Way Merge** (when both branches have new commits):

```
Before:
main:    [C1] ──► [C2] ──► [C3] ──► [C6]
                              \
feature:                       ──► [C4] ──► [C5]

After git merge feature:
main:    [C1] ──► [C2] ──► [C3] ──► [C6] ──► [C7: Merge commit]
                              \                /
feature:                       ──► [C4] ──► [C5]
```

A new merge commit (C7) is created.

```bash
# Merge with always creating a merge commit (no fast-forward)
git merge --no-ff feature-login

# Merge and squash all feature commits into one commit
git merge --squash feature-login
git commit -m "Add login feature"
```

## 🔷 Handling Merge Conflicts

A **merge conflict** happens when two branches changed the same part of the same file differently.

```bash
# When you see this after git merge:
CONFLICT (content): Merge conflict in app.js
Automatic merge failed; fix conflicts and then commit the result.
```

**Inside the conflicted file, you'll see:**

```
<<<<<<< HEAD (your current branch)
const greeting = "Hello World";
=======
const greeting = "Hi Everyone";
>>>>>>> feature-greeting (branch you're merging)
```

**To resolve:**

1. Open the file
2. Decide which code to keep (or combine both)
3. Delete the `<<<<<<<`, `=======`, `>>>>>>>` markers
4. Save the file
5. `git add filename.js`
6. `git commit`

```bash
# See which files have conflicts
git status

# Abort a merge (go back to before the merge)
git merge --abort
```

## 🔷 Rebasing

Rebase **rewrites history** by replaying your commits on top of another branch.

```
Before rebase:
main:    [C1] ──► [C2] ──► [C3] ──► [C6]
                              \
feature:                       ──► [C4] ──► [C5]

After git rebase main (from feature branch):
main:    [C1] ──► [C2] ──► [C3] ──► [C6]
                                          \
feature:                                   ──► [C4'] ──► [C5']
```

```bash
# Switch to feature branch
git checkout feature-login

# Rebase onto main
git rebase main

# Interactive rebase — rewrite, squash, edit last N commits
git rebase -i HEAD~3
```

**In interactive rebase, you'll see:**

```
pick a1b2c3d Add login form
pick e4f5g6h Fix validation
pick i7j8k9l Add error messages

# Commands:
# pick = keep commit as is
# reword = keep commit, edit message
# squash (s) = combine with previous commit
# fixup (f) = combine with previous, discard message
# drop (d) = delete this commit
```

> ⚠️ **Golden Rule of Rebasing:** Never rebase commits that have been pushed to a shared remote branch. It rewrites history and will cause problems for your teammates.

## 🔷 Cherry-Pick

Apply a specific commit from one branch to another:

```bash
# Get the commit hash from git log
git log --oneline

# Apply that specific commit to current branch
git cherry-pick a1b2c3d

# Cherry-pick without auto-committing
git cherry-pick --no-commit a1b2c3d
```

---

# Chapter 7: Working with Remote Repositories

## 🔷 Remote Basics

```bash
# View remote connections
git remote -v

# Add a remote (usually 'origin' = your GitHub repo)
git remote add origin https://github.com/username/repo.git

# Change remote URL (e.g., switching from HTTPS to SSH)
git remote set-url origin git@github.com:username/repo.git

# Remove a remote
git remote remove origin

# Rename a remote
git remote rename origin upstream
```

## 🔷 Pushing (Local → Remote)

```bash
# Push current branch to remote
git push origin branch-name

# First-time push of a new branch (sets upstream tracking)
git push -u origin branch-name
# After this, you can just type 'git push'

# Push all branches
git push --all origin

# Push and delete a remote branch
git push origin --delete branch-name

# Force push (DANGEROUS — overwrites remote history)
git push --force
# Safer force push (fails if someone else pushed)
git push --force-with-lease
```

> ⚠️ **Never force push to main/master in a team without agreement.**

## 🔷 Fetching & Pulling (Remote → Local)

```bash
# Fetch: download changes from remote but DON'T merge
git fetch origin

# Fetch all remotes
git fetch --all

# Pull: fetch + merge into current branch
git pull origin main

# Pull with rebase instead of merge
git pull --rebase origin main

# Pull from tracked upstream (after -u was set)
git pull
```

**Fetch vs Pull:**

```
git fetch:  Remote → Local Repository (safe, doesn't touch your working files)
git pull:   Remote → Local Repository → Working Directory (fetch + merge, can cause conflicts)
```

> 💡 **Best Practice:** Use `git fetch` first to see what changed, then decide. Or use `git pull --rebase` to keep history cleaner.

## 🔷 Complete Remote Workflow

```bash
# 1. Clone a repo
git clone git@github.com:username/project.git
cd project

# 2. Create a feature branch
git checkout -b feature/user-profile

# 3. Do your work, stage, commit
git add .
git commit -m "feat: add user profile page"

# 4. Push your feature branch to remote
git push -u origin feature/user-profile

# 5. Go to GitHub → Create Pull Request → Get reviewed → Merge

# 6. Switch back to main and pull latest
git checkout main
git pull origin main

# 7. Delete your local feature branch (cleanup)
git branch -d feature/user-profile
```

---

# Chapter 8: Stashing & Cleaning

## 🔷 Git Stash

Stash temporarily saves your uncommitted changes so you can switch tasks without committing half-done work.

**Scenario:** You're working on a feature. Your boss says "fix this urgent bug NOW." Your feature is half-done. You don't want to commit it. Solution: Stash it!

```bash
# Stash your current uncommitted changes
git stash

# Stash with a descriptive name (highly recommended)
git stash save "work in progress: user authentication"

# Stash including untracked (new) files
git stash -u
# or
git stash --include-untracked

# List all stashes
git stash list
# Output:
# stash@{0}: On feature-login: work in progress: user authentication
# stash@{1}: On main: temp changes

# Apply the most recent stash (keeps stash in list)
git stash apply

# Apply a specific stash
git stash apply stash@{2}

# Apply AND remove from stash list (most common)
git stash pop

# Remove a specific stash
git stash drop stash@{0}

# Remove all stashes
git stash clear

# Create a new branch from a stash
git stash branch new-branch-name stash@{0}

# See what's in a stash without applying
git stash show stash@{0}
git stash show -p stash@{0}   # with diff
```

**Typical stash workflow:**

```bash
git stash save "half-done feature X"      # save current work
git checkout main                          # switch to main
# fix urgent bug
git commit -m "fix: urgent bug"
git push
git checkout feature-x                    # go back
git stash pop                             # restore your work
# continue feature X
```

## 🔷 Git Clean

Removes **untracked files** from working directory.

```bash
# Preview what would be removed (dry run — ALWAYS do this first!)
git clean -n

# Remove untracked files
git clean -f

# Remove untracked files AND directories
git clean -fd

# Remove untracked + ignored files (really clean slate)
git clean -fdx

# Interactive mode (choose what to clean)
git clean -i
```

> ⚠️ **`git clean` cannot be undone!** Always run `git clean -n` first to preview.

---

# Chapter 9: Tagging

Tags mark specific points in history — usually for **releases** (v1.0, v2.0).

```bash
# Create a lightweight tag (just a name, no metadata)
git tag v1.0

# Create an annotated tag (recommended — includes message, author, date)
git tag -a v1.0 -m "Version 1.0 — First stable release"

# Tag a specific past commit
git tag -a v0.9 abc1234 -m "Beta release"

# List all tags
git tag

# List tags matching a pattern
git tag -l "v1.*"

# Show tag details
git show v1.0

# Push a specific tag to remote
git push origin v1.0

# Push ALL tags to remote
git push origin --tags

# Delete a local tag
git tag -d v1.0

# Delete a remote tag
git push origin --delete v1.0

# Check out code at a tag (read-only — detached HEAD)
git checkout v1.0
```

---

# Chapter 10: Undoing Things — The Rescue Chapter

> 🆘 **This chapter will save your job someday.**

## 🔷 Undo Map

| Situation                                         | Command                           |
| ------------------------------------------------- | --------------------------------- |
| Undo changes in working directory (not staged)    | `git restore filename`          |
| Unstage a file (keep changes in working dir)      | `git restore --staged filename` |
| Amend last commit (not pushed)                    | `git commit --amend`            |
| Undo last commit, keep changes staged             | `git reset --soft HEAD~1`       |
| Undo last commit, keep changes unstaged           | `git reset HEAD~1`              |
| Undo last commit, delete changes entirely         | `git reset --hard HEAD~1`       |
| Undo a pushed commit (safe — creates new commit) | `git revert HEAD`               |
| Recover deleted branch/lost commits               | `git reflog`                    |

## 🔷 git restore (new, recommended)

```bash
# Discard changes to a file in working directory
git restore filename.txt

# Discard ALL changes in working directory
git restore .

# Unstage a file (move it out of staging area)
git restore --staged filename.txt

# Restore a file to how it was at a specific commit
git restore --source abc1234 filename.txt
```

## 🔷 git reset

**`git reset` moves HEAD (and the branch pointer) to a different commit.**

```
git reset --soft HEAD~1:
 - Moves HEAD back 1 commit
 - Changes stay STAGED (in staging area)
 - Use case: "I committed too early, I want to add more"

git reset HEAD~1 (or --mixed, the default):
 - Moves HEAD back 1 commit  
 - Changes stay in WORKING DIRECTORY (unstaged)
 - Use case: "I want to re-do my commit differently"

git reset --hard HEAD~1:
 - Moves HEAD back 1 commit
 - Changes are DELETED from working directory
 - Use case: "I want to completely discard this commit"
 - ⚠️ DANGEROUS — changes are gone!
```

```bash
# Undo last 1 commit (keep changes staged)
git reset --soft HEAD~1

# Undo last 1 commit (keep changes, unstaged)
git reset HEAD~1

# Undo last 3 commits (delete changes)
git reset --hard HEAD~3

# Reset to a specific commit
git reset --hard abc1234
```

## 🔷 git revert (safe for shared/pushed commits)

`git revert` creates a **new commit** that undoes the changes — it doesn't rewrite history.

```bash
# Revert the latest commit
git revert HEAD

# Revert a specific commit
git revert abc1234

# Revert without auto-committing
git revert --no-commit abc1234
```

> ✅ Use `git revert` when you've already pushed. Never use `git reset --hard` on pushed commits in a team.

## 🔷 git reflog — The Ultimate Safety Net

`git reflog` records every movement of HEAD. **It can save you from almost any disaster.**

```bash
# See full reflog
git reflog

# Output example:
# abc1234 HEAD@{0}: commit: Add dark mode
# def5678 HEAD@{1}: checkout: moving from main to feature
# ghi9012 HEAD@{2}: commit: Fix login bug

# Recover a lost commit or branch
git checkout HEAD@{2}
# or
git reset --hard HEAD@{2}
```

> 💡 Git keeps reflog for 90 days by default. If you accidentally delete a branch or reset --hard, reflog can bring it back.

---

# Chapter 11: Advanced Commands

## 🔷 git bisect — Find the Bug

Binary search through commits to find which commit introduced a bug.

```bash
# Start bisect
git bisect start

# Mark current commit as bad (has the bug)
git bisect bad

# Mark a known good commit (no bug)
git bisect good v1.0

# Git checks out a middle commit — test it, then tell git:
git bisect good    # if this commit is fine
git bisect bad     # if this commit has the bug

# Git keeps narrowing down until it finds the culprit commit

# End bisect session
git bisect reset
```

## 🔷 git blame — Who Changed What

```bash
# Show who last modified each line of a file
git blame filename.txt

# Show blame with specific line range
git blame -L 10,25 filename.txt

# Ignore whitespace changes
git blame -w filename.txt
```

## 🔷 git reflog

Already covered in Chapter 10 — your ultimate safety net.

## 🔷 git submodule — Repos Inside Repos

When your project depends on another Git repository.

```bash
# Add a submodule
git submodule add https://github.com/user/library.git libs/library

# Clone a repo that has submodules
git clone --recurse-submodules https://github.com/user/project.git

# If you forgot --recurse-submodules:
git submodule update --init --recursive

# Update all submodules to latest
git submodule update --remote
```

## 🔷 git archive — Export Without .git

```bash
# Create a zip of your project (without .git folder)
git archive --format=zip HEAD > project.zip

# Create a tar.gz
git archive --format=tar.gz HEAD > project.tar.gz

# Archive a specific branch or tag
git archive --format=zip v1.0 > v1.0.zip
```

## 🔷 git gc — Garbage Collection

```bash
# Clean up and optimize the repository
git gc

# More aggressive cleanup
git gc --aggressive
```

## 🔷 git shortlog — Summary of Commits

```bash
# Show commit counts by author
git shortlog -s -n
```

## 🔷 Searching with git grep

```bash
# Search for a string in all tracked files
git grep "function login"

# Search and show line numbers
git grep -n "TODO"

# Search in a specific commit
git grep "old-function" abc1234
```

---

# Chapter 12: GitHub-Specific Features

## 🔷 GitHub CLI (gh)

Install: https://cli.github.com

```bash
# Login to GitHub via terminal
gh auth login

# Clone a repository
gh repo clone username/repo

# Create a new GitHub repository
gh repo create my-new-repo --public
gh repo create my-new-repo --private

# View repository in browser
gh repo view --web

# Create a pull request
gh pr create --title "Add login feature" --body "Description here"

# List pull requests
gh pr list

# View a pull request
gh pr view 42

# Checkout a PR locally (for reviewing)
gh pr checkout 42

# Merge a PR
gh pr merge 42

# List issues
gh issue list

# Create an issue
gh issue create --title "Bug: Login fails" --body "Steps to reproduce..."

# View an issue
gh issue view 15
```

## 🔷 Pull Requests (PRs)

A **Pull Request** is a GitHub feature to propose code changes and request review before merging.

**How to create a PR:**

1. Push your feature branch: `git push -u origin feature/login`
2. Go to GitHub → your repo → "Compare & pull request"
3. Set base branch (main) and compare branch (your feature)
4. Add title, description, reviewers, labels
5. Click "Create pull request"
6. Reviewers comment/approve
7. Once approved → "Merge pull request"

**PR best practices:**

* Keep PRs small and focused (one feature/fix per PR)
* Write a clear description of what and why
* Link to related issues: "Closes #42"
* Add screenshots for UI changes
* Respond to all review comments

## 🔷 Forking Workflow (Open Source Contribution)

```bash
# 1. Fork the repo on GitHub (click Fork button)

# 2. Clone YOUR fork
git clone git@github.com:YOUR-username/project.git

# 3. Add original repo as 'upstream'
git remote add upstream git@github.com:original-author/project.git

# 4. Create a branch
git checkout -b fix/typo-in-readme

# 5. Make changes, commit
git add README.md
git commit -m "fix: correct typo in README"

# 6. Push to YOUR fork
git push origin fix/typo-in-readme

# 7. Create PR from YOUR fork → original repo on GitHub

# 8. Keep your fork in sync with original
git fetch upstream
git checkout main
git merge upstream/main
git push origin main
```

## 🔷 GitHub Actions (CI/CD Basics)

GitHub Actions automates tasks like testing, building, deploying your code.

```yaml
# .github/workflows/ci.yml
name: CI

on:
  push:
    branches: [ main ]
  pull_request:
    branches: [ main ]

jobs:
  test:
    runs-on: ubuntu-latest
    steps:
      - uses: actions/checkout@v3
      - name: Setup Node.js
        uses: actions/setup-node@v3
        with:
          node-version: '18'
      - run: npm install
      - run: npm test
```

## 🔷 GitHub Pages (Free Hosting)

Host a static website directly from your GitHub repo:

1. Go to repo → Settings → Pages
2. Source: Deploy from a branch
3. Branch: main, folder: / (root) or /docs
4. Your site is live at `username.github.io/repo-name`

---

# Chapter 13: VSCode + Git Setup

## 🔷 Built-in Git in VSCode

VSCode has Git built in. No extension required for basics.

**Source Control Panel:** Click the branch icon on the left sidebar (or `Ctrl+Shift+G`)

* **Changes:** Modified files (click + to stage, click file to see diff)
* **Staged Changes:** Ready to commit
* **Message box:** Type commit message → `Ctrl+Enter` to commit

## 🔷 Useful VSCode Git Keyboard Shortcuts

| Action              | Shortcut                     |
| ------------------- | ---------------------------- |
| Open Source Control | `Ctrl+Shift+G`             |
| Open terminal       | `Ctrl+``                     |
| Stage current file  | In diff view: stage button   |
| Toggle inline diff  | Click file in Source Control |

## 🔷 Recommended VSCode Extensions for Git

1. **GitLens** (most popular)
   * See who changed each line (inline blame)
   * Rich commit history
   * Compare branches/commits visually
2. **Git Graph**
   * Visual branch graph (like `git log --graph`)
   * Checkout, merge, rebase from GUI
3. **GitHub Pull Requests and Issues**
   * View and create PRs from VSCode
   * Review PRs inline
4. **Git History**
   * Browse file history visually

**Install any extension:** `Ctrl+Shift+X` → search name → Install

## 🔷 VSCode Settings for Git

Open settings (`Ctrl+,`) or settings.json:

```json
{
  "git.autofetch": true,
  "git.confirmSync": false,
  "git.enableSmartCommit": true,
  "gitlens.currentLine.enabled": true,
  "editor.formatOnSave": true
}
```

## 🔷 Using Terminal in VSCode

```bash
# Open integrated terminal
Ctrl+`

# Split terminal
Ctrl+Shift+5

# You can run all git commands here just like any terminal
git status
git add .
git commit -m "feat: add awesome feature"
git push
```

---

# Chapter 14: Real-World Workflows

## 🔷 Git Flow (Popular Team Workflow)

```
main (production-ready code)
  └── develop (integration branch)
        ├── feature/user-auth
        ├── feature/payment
        └── hotfix/critical-bug
```

**Branches:**

* `main` — Only release-ready code. Tagged with versions.
* `develop` — Integration branch. Features merge here.
* `feature/*` — New features. Branch from develop.
* `release/*` — Prep for production. Branch from develop.
* `hotfix/*` — Emergency fixes. Branch from main.

```bash
# Install git-flow CLI (optional helper)
# Mac: brew install git-flow
# Then:
git flow init
git flow feature start login-page
git flow feature finish login-page
git flow release start v1.0
git flow release finish v1.0
```

## 🔷 GitHub Flow (Simpler, Modern)

Simpler than Git Flow. Great for most teams.

```
main  ──► [always deployable]
  └── feature branches → PR → merge to main → deploy
```

**Steps:**

1. `git checkout -b feature/new-thing`
2. Make commits
3. `git push -u origin feature/new-thing`
4. Create Pull Request
5. Get reviewed and approved
6. Merge to main
7. Deploy main

## 🔷 Trunk-Based Development

* Everyone commits to `main` (trunk) frequently
* Feature flags hide unfinished features
* Very short-lived branches (less than a day)
* Used by Google, Facebook

## 🔷 Solo Developer Workflow

```bash
# Morning routine
git pull origin main

# Start a new feature (even for solo work, branches are good habits)
git checkout -b fix/navbar-alignment

# Work...
git add .
git commit -m "fix: align navbar items properly"

# End of day / when done
git push origin fix/navbar-alignment
git checkout main
git merge fix/navbar-alignment
git push origin main
git branch -d fix/navbar-alignment
```

---

# Chapter 15: Common Issues & Fixes

## 🔷 "I committed to main by mistake"

```bash
# Move the commit to a new branch, then reset main
git branch feature/oops         # create branch with your commit
git reset --hard HEAD~1          # remove commit from main
git checkout feature/oops        # go to your branch
# Now create a PR from this branch
```

## 🔷 "I accidentally deleted a branch"

```bash
# Find the commit hash from reflog
git reflog
# Look for the last commit on that branch, e.g. abc1234

# Recreate the branch from that commit
git checkout -b recovered-branch abc1234
```

## 🔷 "I have merge conflicts and I'm scared"

```bash
# Step 1: Stay calm. Open conflicted files.
git status   # shows which files have conflicts

# Step 2: In each file, look for:
# <<<<<<< HEAD
# your changes
# =======
# their changes
# >>>>>>> branch-name

# Step 3: Edit the file — keep what's correct, delete markers

# Step 4: Stage the resolved file
git add resolved-file.txt

# Step 5: Continue the merge
git commit

# If overwhelmed, abort entirely:
git merge --abort
```

## 🔷 "git push rejected — remote has changes"

```bash
# Someone else pushed before you. First pull their changes:
git pull --rebase origin main
# Resolve any conflicts if they appear
git push origin main
```

## 🔷 "I pushed a secret/password to GitHub!"

**Act immediately:**

1. Rotate the secret (change API key/password immediately in the service)
2. Remove from code
3. `git commit -am "remove secret"`
4. `git push`
5. Contact GitHub support to remove from cached views
6. Consider using BFG Repo Cleaner to scrub history

```bash
# Using BFG to remove a file from entire history
# (install BFG first: https://rtyley.github.io/bfg-repo-cleaner/)
bfg --delete-files .env
git push --force
```

> ⚠️ If a secret has been pushed,  **assume it's compromised** . Always rotate it.

## 🔷 "I can't push — permission denied"

```bash
# Check your remote URL
git remote -v

# Test SSH connection
ssh -T git@github.com

# If SSH isn't set up, switch to HTTPS:
git remote set-url origin https://github.com/username/repo.git

# Or set up SSH (see Chapter 3)
```

## 🔷 "My working directory is a mess — I want to start fresh"

```bash
# See what's changed
git status

# Option 1: Discard all unstaged changes
git restore .

# Option 2: Discard everything (staged AND unstaged)
git reset --hard HEAD

# Option 3: Also remove untracked files
git reset --hard HEAD
git clean -fd
```

## 🔷 "git pull fails with 'refusing to merge unrelated histories'"

```bash
git pull origin main --allow-unrelated-histories
```

## 🔷 "Line ending issues between Windows and Mac/Linux"

```bash
# On Windows: convert to Windows line endings on checkout
git config --global core.autocrlf true

# On Mac/Linux: convert to Unix line endings on commit
git config --global core.autocrlf input
```

## 🔷 "I want to see what a file looked like N commits ago"

```bash
# Show file content at a specific commit
git show HEAD~3:src/app.js

# Or checkout the file from that commit
git restore --source HEAD~3 src/app.js
```

## 🔷 "Git is asking for password every time I push"

```bash
# Cache credentials for 1 hour (3600 seconds)
git config --global credential.helper cache

# Cache credentials for 24 hours
git config --global credential.helper 'cache --timeout=86400'

# On Windows: use Windows Credential Manager
git config --global credential.helper manager

# Best solution: Set up SSH (Chapter 3)
```

---

# Chapter 16: Quick Reference Cheatsheet

## 🔷 Setup

```bash
git config --global user.name "Name"
git config --global user.email "email"
git init
git clone <url>
```

## 🔷 Daily Workflow

```bash
git status
git add .
git commit -m "message"
git push
git pull
```

## 🔷 Branching

```bash
git branch                         # list
git checkout -b feature            # create + switch
git switch feature                 # switch
git merge feature                  # merge
git branch -d feature              # delete
```

## 🔷 Remote

```bash
git remote -v                      # view remotes
git remote add origin <url>        # add remote
git push -u origin branch          # first push
git push                           # subsequent pushes
git pull                           # fetch + merge
git fetch                          # fetch only
```

## 🔷 Undoing

```bash
git restore file.txt               # discard working dir changes
git restore --staged file.txt      # unstage
git commit --amend                 # fix last commit
git reset --soft HEAD~1            # undo commit, keep staged
git reset HEAD~1                   # undo commit, keep unstaged
git reset --hard HEAD~1            # undo commit, delete changes
git revert HEAD                    # safe undo (creates new commit)
git reflog                         # recover anything
```

## 🔷 Stash

```bash
git stash                          # save current work
git stash pop                      # restore + remove from stash
git stash list                     # show all stashes
git stash drop stash@{0}          # delete a stash
```

## 🔷 History & Inspection

```bash
git log --oneline --graph --all    # pretty history
git diff                           # working dir changes
git diff --staged                  # staged changes
git blame file.txt                 # who changed each line
git show abc1234                   # show a commit
```

## 🔷 Tags

```bash
git tag -a v1.0 -m "message"      # create tag
git push origin --tags             # push tags
git tag -d v1.0                    # delete tag
```

---

## 🔷 Git Concepts Mental Model

```
REMOTE REPO (GitHub)
      ↑  git push
      ↓  git fetch/pull
LOCAL REPO (.git folder)
      ↑  git commit
      ↓  git reset
STAGING AREA (Index)
      ↑  git add
      ↓  git restore --staged
WORKING DIRECTORY (your files)
      ↑  you edit files
      ↓  git restore
ORIGINAL STATE
```

---

## 🔷 Branch Strategy Summary

```
main        ──────────────────────────────► (always production-ready)
               ↑ merge via PR
develop     ──────────────────────────────► (integration)
               ↑ merge when feature done
feature/*   ──► work ──► work ──► done    (short-lived, one feature)
hotfix/*    ──► quick fix ──► done        (urgent production fix)
```

---

*Last Updated: 2025*
*Contribute improvements via PR!*
