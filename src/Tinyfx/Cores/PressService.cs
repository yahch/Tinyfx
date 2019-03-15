using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tinyfx.Common;
using Tinyfx.Crypto;
using Tinyfx.Models;
using Tinyfx.Utils;

namespace Tinyfx.Cores
{
    /// <summary>
    /// 文章数据服务类
    /// </summary>
    public sealed class PressService
    {

        private Faes faes = new Faes();

        private ISerializor _serializer = TinyfxCore.SERIALIZER;

        /// <summary>
        /// 数据库文件名
        /// </summary>
        private static readonly string _dbFileName = TinyfxCore.DbFilename;

        public List<Post> AllPosts
        {
            get
            {
                List<Post> posts = null;
                try
                {
                    posts = TStorage.GetInstance()["_POSTS"] as List<Post>;
                }
                catch(Exception ex)
                {
                    LogHelper.WriteLog(LogHelper.LogType.ERROR, ex.Message, ex);
                    posts = null;
                }
                if (posts == null)
                {
                    posts = ReadPostsFromDatabase();
                    TStorage.GetInstance()["_POSTS"] = posts;
                }
                return posts;
            }
        }

        public void EmptyPostsCache()
        {
            try
            {
                TStorage.GetInstance()["_POSTS"] = null;
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogType.ERROR, ex.Message, ex);
            }
        }

        /// <summary>
        /// 从数据库读取所有文章
        /// </summary>
        /// <returns></returns>
        public List<Post> ReadPostsFromDatabase()
        {
            if (!File.Exists(_dbFileName))
            {
                return new List<Post>();
            }
            FileInfo finfo = new FileInfo(_dbFileName);
            if (finfo.Length < 10)
            {
                return new List<Post>();
            }
            string xml = File.ReadAllText(_dbFileName);
            return _serializer.DeserializorFromString<List<Post>>(xml).OrderByDescending(s => s.Id).ToList();
        }


        public PressService()
        {

        }

        /// <summary>
        /// 将文章写回数据库
        /// </summary>
        /// <param name="list"></param>
        private void WritePostsToDatabase(List<Post> list)
        {
            string xml = _serializer.SerializorToString(list);
            string tmp = _dbFileName + "." + Guid.NewGuid().ToString();
            TryExecute.Execute(()=> 
            {
                File.WriteAllText(tmp, xml);
            });
            if (File.Exists(_dbFileName + ".bak"))
            {
                TryExecute.Execute(()=> 
                {
                    File.Delete(_dbFileName + ".bak");
                });
            }
            TryExecute.Execute(()=> 
            {
                File.Copy(_dbFileName, _dbFileName + ".bak");
            });
            TryExecute.Execute(()=> 
            {
                File.Delete(_dbFileName);
            });
            TryExecute.Execute(()=> 
            {
                File.Move(tmp, _dbFileName);
            });
            TryExecute.Execute(()=> 
            {
                File.Delete(tmp);
            });
            EmptyPostsCache();

        }


        /// <summary>
        /// 获取指定页的文章
        /// </summary>
        /// <param name="page"></param>
        /// <param name="maxPage"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public List<Post> GetPostInPage(int page, ref int maxPage, bool includeDraft, int pageSize = 5)
        {
            List<Post> postsList = new List<Post>();
            maxPage = (int)Math.Ceiling(AllPosts.Count() / (double)pageSize);
            List<Post> postsQuery = new List<Post>();
            if (!includeDraft)
            {
                postsQuery = AllPosts.Where(s => s.Visible == true).Skip((page - 1) * pageSize).Take(pageSize).ToList();
            } else
            {
                postsQuery = AllPosts.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            if (TinyfxCore.Configuration.Encryption)
            {
                foreach (var item in postsQuery)
                {
                    var citem = new Post();
                    citem.Id = item.Id;
                    citem.Visible = item.Visible;
                    citem.Content = faes.Decrypt(item.Content);
                    citem.Title = faes.Decrypt(item.Title);
                    postsList.Add(citem);
                }

                return postsList;
            }
            else
            {
                return postsQuery;
            }

        }

        /// <summary>
        /// 删除文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool DeletePost(long id)
        {
            var posts = ReadPostsFromDatabase();
            try
            {
                Post post = posts.First(s => s.Id == id);
                if (post == null)
                {
                    throw new TinyException("记录不存在");
                }
                posts.Remove(post);
                WritePostsToDatabase(posts);
                return true;
            }
            catch(Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogType.ERROR, ex.Message, ex);
                return false;
            }
        }

        /// <summary>
        /// 根据ID获取文章
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Post GetPostByID(long id,bool includeDraft)
        {
            try
            {
                Post post = null;
                if (!includeDraft)
                {
                    post = AllPosts.Where(s => s.Visible == true).FirstOrDefault(s => s.Id == id);
                }
                else
                {
                    post = AllPosts.FirstOrDefault(s => s.Id == id);
                }
                if (TinyfxCore.Configuration.Encryption)
                {
                    var p = new Post();
                    p.Title = faes.Decrypt(post.Title);
                    p.Content = faes.Decrypt(post.Content);
                    p.Visible = post.Visible;
                    p.Id = post.Id;
                    return p;
                }
                else
                {
                    return post;
                }
            }
            catch (Exception ex)
            {
                LogHelper.WriteLog(LogHelper.LogType.ERROR, ex.Message, ex);
                return null;
            }
        }

        /// <summary>
        /// 设置文章隐藏或可见
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool SwitchVisibleByID(long id)
        {
            int vindex = ReadPostsFromDatabase().FindIndex(s => s.Id == id);
            if (vindex < 0) return false;
            var spost = AllPosts[vindex];
            spost.Visible = !spost.Visible;
            AllPosts[vindex] = spost;
            WritePostsToDatabase(AllPosts);
            return true;
        }

        /// <summary>
        /// 增加或修改文章
        /// </summary>
        /// <param name="post"></param>
        /// <returns></returns>
        public bool InsertOrUpdatePost(Post post)
        {
            if (TinyfxCore.Configuration.Encryption)
            {
                post.Content = faes.Encrypt(post.Content);
                post.Title = faes.Encrypt(post.Title);
            }
            var posts = ReadPostsFromDatabase();
            if (posts.Count() > 0)
            {
                if (post.Id > 0)
                {
                    var sindex = ReadPostsFromDatabase().FindIndex(s => s.Id == post.Id);
                    if (sindex < 0)
                    {
                        return false;
                    }
                    var spost = posts[sindex];
                    spost.Content = post.Content;
                    spost.Title = post.Title;
                    spost.Visible = post.Visible;
                    posts[sindex] = spost;
                }
                else
                {
                    post.Id = DateTime.Now.Ticks;
                    posts.Insert(0, post);
                }
            }
            else
            {
                post.Id = DateTime.Now.Ticks;
                posts.Add(post);
            }
            WritePostsToDatabase(posts);
            return true;
        }
    }
}
