var azure = require('azure'),
    uuid = require('node-uuid'),
    tableName = 'blog',
    partitionKey = 'entries',
    tableService = azure.createTableService();

var Post = exports = module.exports = function Post(title, body) {
    this.PartitionKey = partitionKey;
    this.RowKey = uuid.v4();
    this.title = title;
    this.body = body;
    this.createdAt = new Date;
};

function ensureTableExists(fn){
    tableService.createTableIfNotExists(tableName, function(error) {
	if(error){
	    return fn(new Error('Could not use storage table'));
	}
	fn();
    });
};

function unwrapEntity(entity){
    return {
	PartitionKey: entity.PartitionKey,
	RowKey: entity.RowKey,
	title: entity.title,
	body: entity.body,
	createdAt: entity.createdAt
    };
}

function wrapEntities(entities){
    if(entities.length){
	var result = [];
	for(var i = 0; i < entities.length; i++){
	    result[i] = wrapEntity(entities[i]);
	}
	return result;
    } else {
	return wrapEntity(entities);
    }
    
    function wrapEntity(entity){
	var post = new Post(entity.title, entity.body);
	post.RowKey = entity.RowKey;
	post.PartitionKey = entity.PartitionKey;
	post.createdAt = entity.createdAt;
	return post;
    }
}

Post.prototype.save = function(fn){
    var entity = unwrapEntity(this);
    ensureTableExists(function(error){
	if(error){
	    return fn(error);
	}
	tableService.insertEntity(tableName, entity, function(error){
	    if(error){
		return fn(new Error('Could not create blog post'));
	    }
	    fn();
	});
    });
};

Post.prototype.validate = function(fn){
    if (!this.title) return fn(new Error('_title_ required'));
    if (!this.body) return fn(new Error('_body_ required'));
    if (this.body.length < 10) {
	return fn(new Error(
            '_body_ should be at least **10** characters long, was only _' + 
		this.title.length + '_'));
    }
    fn();
};

Post.prototype.update = function(data, fn){
    this.updatedAt = new Date;
    for (var key in data) {
	if (undefined != data[key]) {
	    this[key] = data[key];
	}
    }
    var entity = unwrapEntity(this);
    ensureTableExists(function(error){
	if(error){
	    return fn(error);
	}
 	tableService.updateEntity(tableName, entity, function(error){
	    if(error)
	    {
		return fn(new Error('Could not update blog post'));
	    }
	    fn();
	});
    });
};

Post.prototype.destroy = function(fn){
    exports.destroy(this.id, fn);
};

exports.count = function(fn){
    exports.all(function(error, entities){
	if(error){
	    return fn(error);
	}
	fn(null, entities.length);
    });
};

exports.all = function(fn){
    ensureTableExists(function(error){
	if(error)
	{
	    return fn(error);
	}
	tableService.queryEntities(azure.TableQuery.select().from(tableName),
	    function(error, entities){
		if(error){
		    return fn(new Error('Could not query table for all posts'));
		}
		fn(null, wrapEntities(entities));
	    });
    });
};
    

exports.get = function(id, fn){
    ensureTableExists(function(error){
	if(error){
	    return fn(error);
	}
	tableService.queryEntity(tableName, partitionKey, id,
	    function(error, entity){
		if(error){
		    return fn(new Error('Could not query table for this post'));
		}
		fn(null, wrapEntities(entity));
	    });
    });
};

exports.destroy = function(id, fn) {
    ensureTableExists(function(error){
	if(error){
	    return fn(error);
	}
	tableService.deleteEntity(tableName, {PartitionKey: partitionKey,
	    RowKey: id}, function(error){
		if(error){
		    return fn(new Error('Could not delete post'));
		}
		fn();
	    });
    });
};