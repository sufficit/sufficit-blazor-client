.PHONY: all deploy clean

all: src/wwwroot src/wwwroot/index.html

src/wwwroot:
	git worktree add src/wwwroot gh-pages

# Replace this rule with whatever builds your project
src/wwwroot/index.html: src/wwwroot/index.html
	cp $< $@

deploy: all
	cd src/wwwroot && \
	git add --all && \
	git commit -m "Deploy to gh-pages" && \
	git push origin gh-pages

# Removing the actual dist directory confuses git and will require a git worktree prune to fix
clean:
	rm -rf src/wwwroot/*