---
title:Writing an Article
author:Stephen Tafoya
postDate:20200129
description:A simple intro to writing an article
---

# Writing an Article

----

Writing an article is not always the easiest thing ever, and this post is no exception.

## For Starters
### General Formatting
You should always make user of headings, headings help determine different sections in your article.

> This is of course assuming you write complex articles

### Inserting Code

You can insert code by doing the following:

<pre><code data-language="csharp">// The best person class ever written
public class Person
{
    public string FirstName { get; set; }
    public string Lastname { get; set; }
    public int Gender { get; set; }
    public DateTime Age { get; set; }

    public Person()
    {}

    public string GetName()
    {
        return $"{FirstName} {Lastname}";
    }
}
</code></pre>

### Adding an image

you can add an image by using the following syntax:

    ![Cover Image](/Blog/how-to-article/cover.png)

which results in an image similar to what is below

![Cover Image](/Blog/how-to-article/cover.png)

### Creating a list

- first element of list
- second element of list
- third element of list
    - sibling of third element
        - sibling of third first element
- fourth element of list

### Final Words
In summation, This was a very short temp blog post.

I will continue to expand it but that is only for development purposes.

Thank you,